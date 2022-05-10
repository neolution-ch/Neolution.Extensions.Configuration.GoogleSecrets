namespace Neolution.Extensions.Configuration.GoogleSecrets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleSecretsProvider"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public GoogleSecretsProvider(GoogleSecretsSource source)
        {
            this.Source = source;
            this.logger = LoggerFactory.Create(b => b.AddConsole()).CreateLogger<GoogleSecretsProvider>();
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
                        if (!this.Source.FilterFn(secret))
                        {
                            continue;
                        }

                        string version = "latest";
                        var secretId = secret.SecretName.SecretId;

                        if (this.Source.VersionDictionary?.ContainsKey(secretId) == true)
                        {
                            version = this.Source.VersionDictionary[secretId];
                        }

                        try
                        {
                            string versionName = $"{secret.Name}/versions/{version}";
                            var value = secretManagerServiceClient.AccessSecretVersion(versionName);
                            var secretValue = value.Payload.Data.ToStringUtf8();

                            this.Set(this.Source.MapFn(secret), secretValue);
                            this.logger.LogInformation($"Successfully loaded secret {secret.SecretName.SecretId}");
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
        }
    }
}
