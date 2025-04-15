namespace Neolution.Extensions.Configuration.GoogleSecrets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.WebSockets;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Google.Api.Gax;
    using Google.Api.Gax.ResourceNames;
    using Google.Cloud.SecretManager.V1;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The Google Secrets Provider
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Configuration.ConfigurationProvider" />
    public class GoogleSecretsProvider : ConfigurationProvider
    {
        private readonly ILogger<GoogleSecretsProvider> logger;
        private readonly IConfigurationRoot existingConfiguration;
        private readonly Dictionary<string, string> secretCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleSecretsProvider"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="existingConfiguration">The configuration builder.</param>
        public GoogleSecretsProvider(GoogleSecretsSource source, IConfigurationRoot existingConfiguration)
        {
            this.Source = source;
            this.logger = LoggerFactory.Create(b => b.AddConsole()).CreateLogger<GoogleSecretsProvider>();
            this.existingConfiguration = existingConfiguration;
            this.secretCache = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the source.
        /// </summary>
        public GoogleSecretsSource Source { get; }

        /// <inheritdoc />
        public override void Load()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(this.Source.ProjectName))
                {
                    this.logger.LogWarning("ProjectName is not set. Skipping Google Secrets Provider.");
                    return;
                }

                // Create client
                SecretManagerServiceClient secretManagerServiceClient = SecretManagerServiceClient.Create();

                // Initialize request argument(s)
                ListSecretsRequest request = new ListSecretsRequest
                {
                    ParentAsProjectName = ProjectName.FromProject(this.Source.ProjectName),
                    Filter = this.Source.Filter,
                };

                // Make the request
                PagedEnumerable<ListSecretsResponse, Secret> response = secretManagerServiceClient.ListSecrets(request);

                // Iterate over pages (of server-defined size), performing one RPC per page (+ an additional RPC per secret that is accessed)
                foreach (ListSecretsResponse page in response.AsRawResponses())
                {
                    foreach (Secret secret in response)
                    {
                        try
                        {
                            if (!this.Source.FilterFn(secret))
                            {
                                continue;
                            }

                            ScanExistingConfiguration(secretManagerServiceClient, secret);
                            ApplyMapFn(secretManagerServiceClient, secret);
                        }
                        catch (Exception e)
                        {
                            this.logger.LogWarning(e, $"Skipping secret {secret.SecretName.SecretId}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Unhandeled Exception");
            }

            void SetSecretValue(SecretManagerServiceClient secretManagerServiceClient, Secret secret, string key, string version)
            {
                string versionName = $"{secret.Name}/versions/{version}";

                if (secretCache.TryGetValue(versionName, out var cachedValue))
                {
                    this.Set(key, cachedValue);
                    this.logger.LogDebug($"Using cached value for secret {secret.SecretName.SecretId} Key: {key} Version: {version}");
                }
                else
                {
                    var value = secretManagerServiceClient.AccessSecretVersion(versionName);
                    var secretValue = value.Payload.Data.ToStringUtf8();
                    secretCache[versionName] = secretValue;
                    this.Set(key, secretValue);
                }

                this.logger.LogInformation($"Successfully loaded secret {secret.SecretName.SecretId} into configuration. Key: {key} Version: {version}");
            }

            void ScanExistingConfiguration(SecretManagerServiceClient secretManagerServiceClient, Secret secret)
            {
                var existingKeyValues = this.existingConfiguration.AsEnumerable();
                // value will be in the format of {GoogleSecret:SecretName} or {GoogleSecret:SecretName:Version}
                var regex = new Regex($@"\{{GoogleSecret:{secret.SecretName.SecretId}(?::(\w+))?}}");
                var keyValuesToReplace = existingKeyValues.Where(x => !string.IsNullOrWhiteSpace(x.Value) && regex.IsMatch(x.Value));

                foreach (var keyValue in keyValuesToReplace)
                {
                    var replaceParams = keyValue.Value.Split(':');
                    string version = "latest";
                    if (replaceParams.Length >= 3)
                    {
                        version = replaceParams[2].Substring(0, replaceParams[2].Length - 1);
                    }

                    SetSecretValue(secretManagerServiceClient, secret, keyValue.Key, version);
                }
            }

            void ApplyMapFn(SecretManagerServiceClient secretManagerServiceClient, Secret secret)
            {
                if (this.Source.MapFn == null)
                {
                    return;
                }

                string version = "latest";
                var secretId = secret.SecretName.SecretId;

                if (this.Source.VersionDictionary?.ContainsKey(secretId) == true)
                {
                    version = this.Source.VersionDictionary[secretId];
                }

                SetSecretValue(secretManagerServiceClient, secret, this.Source.MapFn(secret), version);
            }
        }
    }
}
