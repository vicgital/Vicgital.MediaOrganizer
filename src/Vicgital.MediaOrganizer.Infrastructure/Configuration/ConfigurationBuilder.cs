using Microsoft.Extensions.Configuration;

namespace Vicgital.MediaOrganizer.Infrastructure.Configuration
{
    /// <summary>
    /// Provides static methods for building application configuration by loading settings from JSON files, environment
    /// variables, and Azure App Configuration sources.
    /// </summary>
    /// <remarks>The methods in this class support environment-specific configuration by reading the
    /// 'ASPNETCORE_ENVIRONMENT' environment variable, defaulting to 'dev' if not set. Configuration is automatically
    /// reloaded if the underlying JSON files change at runtime. Use these methods to centralize and standardize
    /// configuration loading across the application.</remarks>
    public static class ConfigurationBuilder
    {


        /// <summary>
        /// Builds an application configuration by loading settings from JSON files and environment variables.
        /// </summary>
        /// <remarks>The method determines the environment by reading the 'ASPNETCORE_ENVIRONMENT'
        /// environment variable. If the variable is not set, it defaults to 'dev'. The configuration includes
        /// 'appsettings.json' (required), an optional 'appsettings.{Environment}.json', and all environment variables.
        /// Changes to the JSON files are reloaded automatically if modified at runtime.</remarks>
        /// <returns>An <see cref="IConfiguration"/> instance containing the combined application settings from
        /// 'appsettings.json', an optional environment-specific JSON file, and environment variables.</returns>
        public static IConfiguration BuildConfiguration()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "dev";

            var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
