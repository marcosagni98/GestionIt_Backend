using Application;
using Infrastructure;

namespace API.DependencyInjection;

/// <summary>
/// Provides extension methods for adding dependency injection services to the IServiceCollection.
/// </summary>
public static class DiCollectionExtensions
{
    /// <summary>
    /// Adds dependency injection services to the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configuration">The IConfiguration to use for settings.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddPresentation(configuration)
            .AddApplication()
            .AddInfrastructure(configuration);
    }
}