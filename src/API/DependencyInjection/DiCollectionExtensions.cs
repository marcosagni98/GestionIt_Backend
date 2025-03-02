using Application;
using Infrastructure;
using System.Runtime.CompilerServices;

namespace API.DependencyInjection;

public static class DiCollectionExtensions
{
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddApplication()
            .AddInfrastructure(configuration);
    }
}
