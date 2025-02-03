namespace Neolution.Extensions.Configuration.GoogleSecrets
{
    using System;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// The Google Secrets Extensions
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Adds the Google secrets to the <see cref="ConfigurationBuilder"/>.
        /// If the GOOGLE_SECRETS_PROJECT environment variable is set, it will be used as the project name.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The IConfigurationBuilder</returns>
        /// <exception cref="System.ArgumentNullException">options</exception>
        public static IConfigurationBuilder AddGoogleSecrets(this IConfigurationBuilder configuration)
        {
            // Configure app configuration to add Google Secrets if environment variable is set
            var googleSecretProject = Environment.GetEnvironmentVariable(EnvironmentVariableNames.GoogleSecretsProject);
            if (!string.IsNullOrWhiteSpace(googleSecretProject))
            {
                return AddGoogleSecrets(configuration, options =>
                {
                    options.ProjectName = googleSecretProject;
                });
            }

            return AddGoogleSecrets(configuration, _ => { });
        }

        /// <summary>
        /// Adds the Google secrets to the <see cref="ConfigurationBuilder"/>.
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

            if (string.IsNullOrWhiteSpace(googleSecretsOptions.ProjectName))
            {
                throw new ArgumentNullException(nameof(googleSecretsOptions.ProjectName));
            }

            configuration.Add(new GoogleSecretsSource(googleSecretsOptions, configuration.Build()));

            return configuration;
        }
    }
}
