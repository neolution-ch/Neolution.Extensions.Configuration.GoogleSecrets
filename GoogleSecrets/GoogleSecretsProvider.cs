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

    /// <summary>
    /// The Google Secrets Provider
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Configuration.ConfigurationProvider" />
    public class GoogleSecretsProvider : ConfigurationProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleSecretsProvider"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public GoogleSecretsProvider(GoogleSecretsSource source)
        {
            this.Source = source;
        }

        /// <summary>
        /// Gets the source.
        /// </summary>
        public GoogleSecretsSource Source { get; }

        /// <inheritdoc />
        public override void Load()
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
                foreach (Secret item in response)
                {
                    if (!this.Source.FilterFn(item))
                    {
                        continue;
                    }

                    string version = "latest";
                    var secretId = item.SecretName.SecretId;

                    if (this.Source.VersionDictionary?.ContainsKey(secretId) == true)
                    {
                        version = this.Source.VersionDictionary[secretId];
                    }

                    string versionName = $"{item.Name}/versions/{version}";
                    var value = secretManagerServiceClient.AccessSecretVersion(versionName);
                    var secret = value.Payload.Data.ToStringUtf8();

                    this.Set(this.Source.MapFn(item), secret);
                }
            }
        }
    }
}
