namespace Neolution.Extensions.Configuration.GoogleSecrets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using static Neolution.Extensions.Configuration.GoogleSecrets.GoogleSecretsOptions;

    /// <summary>
    /// The Google Secrets Source
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Configuration.IConfigurationSource" />
    public class GoogleSecretsSource : IConfigurationSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleSecretsSource"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public GoogleSecretsSource(GoogleSecretsOptions options)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));

            this.MapFn = options.MapFn;
            this.FilterFn = options.FilterFn;
            this.ProjectName = options.ProjectName;
            this.Filter = options.Filter;
            this.VersionDictionary = options.VersionDictionary;
        }

        /// <summary>
        /// Gets or sets the map function.
        /// </summary>
        public MapFnType MapFn { get; set; }

        /// <summary>
        /// Gets or sets the filter function.
        /// </summary>
        public FilterFnType FilterFn { get; set; }

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets the filter.
        /// </summary>
        public string Filter { get; internal set; }

        /// <summary>
        /// Gets or sets the version dictionary.
        /// </summary>
        public IDictionary<string, string> VersionDictionary { get; set; }

        /// <summary>
        /// Builds the <see cref="Microsoft.Extensions.Configuration.IConfigurationProvider" /> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="Microsoft.Extensions.Configuration.IConfigurationBuilder" />.</param>
        /// <returns>
        /// An <see cref="Microsoft.Extensions.Configuration.IConfigurationProvider" />
        /// </returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new GoogleSecretsProvider(this, builder);
        }
    }
}
