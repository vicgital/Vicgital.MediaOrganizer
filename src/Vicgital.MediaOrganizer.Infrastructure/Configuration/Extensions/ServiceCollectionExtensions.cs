using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vicgital.MediaOrganizer.Infrastructure.Configuration.Services;

namespace Vicgital.MediaOrganizer.Infrastructure.Configuration.Extensions
{
    /// <summary>
    /// Provides extension methods for registering application configuration services with an IServiceCollection.
    /// </summary>
    /// <remarks>This class contains extension methods that simplify the setup of application configuration
    /// dependencies in an ASP.NET Core application's dependency injection container.</remarks>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds application configuration services to the specified service collection.
        /// </summary>
        /// <remarks>Registers the provided configuration instance and the application configuration
        /// abstraction as singletons. This method is typically called during application startup to make configuration
        /// settings available via dependency injection.</remarks>
        /// <param name="services">The service collection to which the configuration services will be added. Cannot be null.</param>
        /// <param name="configuration">The application configuration to register. Cannot be null.</param>
        /// <returns>The service collection with application configuration services registered. This enables further chaining of
        /// service configuration calls.</returns>
        public static IServiceCollection AddAppConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);
            services.AddSingleton<IAppConfiguration, AppConfiguration>();
            return services;
        }

    }
}
