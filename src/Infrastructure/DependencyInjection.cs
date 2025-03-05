using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Utils;
using Infrastructure.Repositories;
using Infrastructure.Utils;

namespace Infrastructure;

/// <summary>
/// Provides extension methods for adding persistance-related services to the dependency injection container
/// </summary>
public static class InfrastructureCollectionExtensions
{
    /// <summary>
    /// Adds all the necessary repositories, dbContext, and unitOfWork for the application
    /// </summary>
    /// <param name="services">The dependency injection container</param>
    /// <param name="configuration">The configuration interface</param>
    /// <returns>The dependency injection container with the persistance services added</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDbContext(configuration)
            .AddRepositories()
            .AddUnitOfWork()
            .AddHelpers();
    }

    /// <summary>
    /// Adds the application's DbContext to the dependency injection container
    /// </summary>
    /// <param name="services">The dependency injection container</param>
    /// <param name="configuration">The configuration interface</param>
    /// <returns>The dependency injection container with dbContext</returns>
    private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        // Build the connection string from environment variables
        var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
        var dbName = Environment.GetEnvironmentVariable("DB_NAME");
        var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
        var dbUser = Environment.GetEnvironmentVariable("DB_USER");
        var connectionString = $"Data Source={dbHost}; Initial Catalog={dbName};User ID={dbUser};Password={dbPassword};TrustServerCertificate=True;";

        return services.
            AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));
    }

    /// <summary>
    /// Adds the application's repositories to the dependency injection container
    /// </summary>
    /// <param name="services">The dependency injection container</param>
    /// <returns>The dependency injection container with repositories</returns>
    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<IIncidentHistoryRepository, IncidentHistoryRepository>()
            .AddScoped<IIncidentRepository, IncidentRepository>()
            .AddScoped<IMessageRepository, MessageRepository>()
            .AddScoped<IUserFeedbackRepository, UserFeedbackRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IWorkLogRepository, WorkLogsRepository>()
            .AddTransient<IEmailSender, EmailSender>();
    }

    /// <summary>
    /// Adds the unit of work to the dependency injection container
    /// </summary>
    /// <param name="services">The dependency injection container</param>
    /// <returns>The dependency injection container with unit of work</returns>
    private static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        return services
            .AddScoped<IUnitOfWork, UnitOfWork>();
    }

    /// <summary>
    /// Registers helper services, including background services such as the database initializer.
    /// </summary>
    /// <param name="services">The service collection to add dependencies to.</param>
    /// <returns>The updated service collection.</returns>
    private static IServiceCollection AddHelpers(this IServiceCollection services)
    {
        return services
             .AddHostedService<DatabaseInitializer>();
    }
}
