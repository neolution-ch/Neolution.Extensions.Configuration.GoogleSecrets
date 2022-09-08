namespace Neolution.AspNetCore.GoogleSecrets
{
    using Microsoft.AspNetCore.Hosting;
    using Neolution.Extensions.Configuration.GoogleSecrets;

    /// <summary>
    /// Google Secrets extensions for <see cref="IWebHostBuilder"/>.
    /// </summary>
    public static class WebHostBuilderExtensions
    {
        /// <summary>
        /// Gets the Google Secrets project identifier.
        /// </summary>
        private static string GoogleSecretsProjectId => Environment.GetEnvironmentVariable("GOOGLE_SECRETS_PROJECT_ID") ?? string.Empty;

        /// <summary>
        /// Gets a value indicating whether to load Google Secrets.
        /// </summary>
        /// <value>
        ///   <c>true</c> to load Google Secrets; otherwise, <c>false</c>.
        /// </value>
        private static bool LoadGoogleSecrets => !string.IsNullOrWhiteSpace(GoogleSecretsProjectId);

        /// <summary>
        /// Configures the Google Secrets in the web host.
        /// </summary>
        /// <param name="webHostBuilder">The web host builder.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder ConfigureGoogleSecrets(this IWebHostBuilder webHostBuilder)
        {
            if (webHostBuilder is null)
            {
                throw new ArgumentNullException(nameof(webHostBuilder));
            }

            return webHostBuilder.ConfigureAppConfiguration((_, configBuilder) =>
            {
                if (LoadGoogleSecrets)
                {
                    configBuilder.AddGoogleSecrets(options =>
                    {
                        options.ProjectName = GoogleSecretsProjectId;
                    });
                }
            });
        }
    }
}
