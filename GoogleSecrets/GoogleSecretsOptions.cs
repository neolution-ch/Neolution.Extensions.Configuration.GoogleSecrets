namespace Neolution.Extensions.Configuration.GoogleSecrets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Google.Cloud.SecretManager.V1;

    /// <summary>
    /// Options Class for the Google Secrets Configuration Provider
    /// </summary>
    public class GoogleSecretsOptions
    {
        /// <summary>
        /// The map function. Used to transform a Secret to a IConfiguration Key
        /// </summary>
        /// <param name="secret">The secret.</param>
        /// <returns>The IConfiguration Key</returns>
        public delegate string MapFnType(Secret secret);

        /// <summary>
        /// The filter Function. Used to filter the Secrets.
        /// </summary>
        /// <param name="secret">The secret.</param>
        /// <returns>True if the secret should be exposed, false otherwise</returns>
        public delegate bool FilterFnType(Secret secret);

        /// <summary>
        /// Gets or sets the map function.
        /// </summary>
        public MapFnType MapFn { get; set; } = secret => secret.SecretName.SecretId.Replace("__", ":");

        /// <summary>
        /// Gets or sets the filter function.
        /// </summary>
        public FilterFnType FilterFn { get; set; } = secret => true;

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>
        /// The name of the project or the numeric project id
        /// </value>
        public string ProjectName { get; set; } = Environment.GetEnvironmentVariable(EnvironmentVariableNames.GoogleSecretsProject) ?? string.Empty;

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>
        /// According to https://cloud.google.com/secret-manager/docs/filtering
        /// </value>
        public string Filter { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the version dictionary.
        /// </summary>
        /// <value>
        /// By default the latest version will be taken but you can request a specific version by
        /// suppling a dictionary entry in the form of SecretId, Version
        /// </value>
        public IDictionary<string, string> VersionDictionary { get; set; }
    }
}
