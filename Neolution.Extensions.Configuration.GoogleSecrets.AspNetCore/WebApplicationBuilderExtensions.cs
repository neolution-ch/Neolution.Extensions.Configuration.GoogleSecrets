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

            builder.Configuration.AddGoogleSecrets(_ => { });
        }

        /// <summary>
        /// Adds the Google secrets to the <see cref="WebApplicationBuilder"/>.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="googleSecretsOptions"></param>
        public static void AddGoogleSecrets(this WebApplicationBuilder builder, Action<GoogleSecretsOptions> googleSecretsOptions)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(googleSecretsOptions);

            builder.Configuration.AddGoogleSecrets(googleSecretsOptions);
        }
    }
}