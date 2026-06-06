using Microsoft.Extensions.Configuration;

namespace Vicgital.MediaOrganizer.Infrastructure.Configuration.Services
{
    /// <summary>
    /// Provides access to application configuration values using strongly-typed retrieval methods.
    /// </summary>
    /// <remarks>This class offers convenient methods for retrieving configuration values as strings or
    /// integers, with support for default values. It is typically used to centralize configuration access within an
    /// application.</remarks>
    /// <param name="configuration">The configuration source used to retrieve application settings. Cannot be null.</param>
    public class AppConfiguration(IConfiguration configuration) : IAppConfiguration
    {
        private readonly IConfiguration _configuration = configuration;

        /// <summary>
        /// Retrieves the value associated with the specified configuration key.
        /// </summary>
        /// <param name="key">The key of the configuration setting to retrieve. Cannot be null or empty.</param>
        /// <returns>The value associated with the specified key.</returns>
        /// <exception cref="ArgumentException">Thrown if the specified key is missing or empty in the configuration.</exception>
        public string GetValue(string key) =>
            _configuration[key] ?? throw new ArgumentException($"Configuration key '{key}' is missing or empty.");

        /// <summary>
        /// Retrieves the value associated with the specified configuration key, or returns a default value if the key
        /// is not found.
        /// </summary>
        /// <param name="key">The configuration key whose value to retrieve. Cannot be null.</param>
        /// <param name="defaultValue">The value to return if the specified key does not exist in the configuration.</param>
        /// <returns>The value associated with the specified key if found; otherwise, the specified default value.</returns>
        public string GetValue(string key, string defaultValue) =>
            _configuration[key] ?? defaultValue;

        /// <summary>
        /// Retrieves the integer value associated with the specified configuration key, or returns a default value if
        /// the key is not found or cannot be parsed as an integer.
        /// </summary>
        /// <param name="key">The configuration key whose value to retrieve. Cannot be null.</param>
        /// <param name="defaultValue">The value to return if the configuration key is not found or cannot be parsed as an integer.</param>
        /// <returns>The integer value associated with the specified key, or the specified default value if the key is not found
        /// or is not a valid integer.</returns>
        public int GetValue(string key, int defaultValue)
        {
            var value = _configuration[key];
            return int.TryParse(value, out var result) ? result : defaultValue;
        }
    }
}
