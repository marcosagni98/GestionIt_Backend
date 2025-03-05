using Application.Helpers.Mappers;
using Application.Interfaces.Services;
using Application.Interfaces.Utils;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

/// <summary>
/// Provides extension methods for adding application-related services to the dependency injection container
/// </summary>
public static class ApplicationCollectionExtensions
{
    /// <summary>
    /// Adds all the necessary application services to the dependency injection container
    /// </summary>
    /// <param name="services">The dependency injection container</param>
    /// <returns>The dependency injection container with the application services added</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services
            .AddServices()
            .AddHelpers();
    }

    /// <summary>
    /// Adds the application's services to the dependency injection container
    /// </summary>
    /// <param name="services">The dependency injection container</param>
    /// <returns>The dependency injection container with the services added</returns>
    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
        .AddScoped<IIncidentHistoryService, IncidentHistoryService>()
        .AddScoped<IIncidentService, IncidentService>()
        .AddScoped<IMessageService, MessageService>()
        .AddScoped<IUserFeedbackService, UserFeedbackService>()
        .AddScoped<IUserService, UserService>()
        .AddScoped<IWorkLogService, WorkLogService>()
        .AddScoped<IStatisticsService, StatisticsService>()
        .AddScoped<IAuthService, AuthService>()
        .AddHttpClients();
    }

    /// <summary>
    /// Adds the application's HTTP clients to the dependency injection container
    /// </summary>
    /// <param name="services">The dependency injection container</param>
    /// <returns>The dependency injection container with the HTTP clients added</returns>
    private static IServiceCollection AddHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<IOllamaService, OllamaService>();
        return services;
    }

    /// <summary>
    /// Adds the application's helper services to the dependency injection container
    /// </summary>
    /// <param name="services">The dependency injection container</param>
    /// <returns>The dependency injection container with the helper services added</returns>
    private static IServiceCollection AddHelpers(this IServiceCollection services)
    {
        return services
            .AddAutomappers()
            .AddUtils();
    }

    /// <summary>
    /// Adds the application's AutoMapper profiles to the dependency injection container
    /// </summary>
    /// <param name="services">The dependency injection container</param>
    /// <returns>The dependency injection container with the AutoMapper profiles added</returns>
    private static IServiceCollection AddAutomappers(this IServiceCollection services)
    {
        return services
            .AddAutoMapper(typeof(UserMapper))
            .AddAutoMapper(typeof(IncidentHistoryMapper))
            .AddAutoMapper(typeof(IncidentMapper))
            .AddAutoMapper(typeof(UserFeedbackMapper))
            .AddAutoMapper(typeof(MessageMapper))
            .AddAutoMapper(typeof(WorkLogMapper));
    }

    /// <summary>
    /// Adds the application's utility services to the dependency injection container
    /// </summary>
    /// <param name="services">The dependency injection container</param>
    /// <returns>The dependency injection container with the utility services added</returns>
    private static IServiceCollection AddUtils(this IServiceCollection services)
    {
        return services
            .AddScoped<IJwt, Jwt>();
    }
}

