namespace Neolution.Extensions.Configuration.GoogleSecrets.AspNetCore
{
    using System;
    using Microsoft.AspNetCore.Builder;

    /// <summary>
    /// Google Secrets extensions for <see cref="WebApplicationBuilder"/>.
    /// </summary>
    public static class WebApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds the Google secrets to the <see cref="WebApplicationBuilder"/>.
        /// Uses the GOOGLE_SECRETS_PROJECT environment variable as the project name.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void AddGoogleSecrets(this WebApplicationBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            // Configure app configuration to add Google Secrets if environment variable is set
            var googleSecretProject = Environment.GetEnvironmentVariable(EnvironmentVariableNames.GoogleSecretsProject);
            if (!string.IsNullOrWhiteSpace(googleSecretProject))
            {
                builder.Configuration.AddGoogleSecrets(options =>
                {
                    options.ProjectName = googleSecretProject;
                });
            }
        }
    }
}