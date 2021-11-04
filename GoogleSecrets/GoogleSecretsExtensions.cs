namespace Neolution.Extensions.Configuration.GoogleSecrets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// The Google Secrets Extensions
    /// </summary>
    public static class GoogleSecretsExtensions
    {
        /// <summary>
        /// Adds the google secrets.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="options">The options.</param>
        /// <returns>The IConfigurationBuilder</returns>
        /// <exception cref="System.ArgumentNullException">options</exception>
        public static IConfigurationBuilder AddGoogleSecrets(this IConfigurationBuilder configuration, Action<GoogleSecretsOptions> options)
        {
            _ = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _ = options ?? throw new ArgumentNullException(nameof(options));

            var googleSecretsOptions = new GoogleSecretsOptions();
            options(googleSecretsOptions);
            configuration.Add(new GoogleSecretsSource(googleSecretsOptions));

            return configuration;
        }
    }
}
