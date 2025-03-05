using Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

/// <summary>
/// Background service responsible for initializing the database at application startup.
/// Ensures the database is created and its tables exist before the application starts running.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DatabaseInitializer"/> class.
/// </remarks>
/// <param name="serviceProvider">The service provider to resolve dependencies.</param>
/// <param name="logger">The logger instance for logging operations.</param>
public class DatabaseInitializer(IServiceProvider serviceProvider, ILogger<DatabaseInitializer> logger) : IHostedService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<DatabaseInitializer> _logger = logger;

    /// <summary>
    /// Starts the database initialization process asynchronously when the application starts.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            if (context.Database.GetService<IDatabaseCreator>() is RelationalDatabaseCreator databaseCreator)
            {
                if (!databaseCreator.CanConnect())
                    await databaseCreator.CreateAsync(cancellationToken);
                if (!databaseCreator.HasTables())
                    await databaseCreator.CreateTablesAsync(cancellationToken);
            }

            _logger.LogInformation("Database successfully initialized.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database.");
        }
    }

    /// <summary>
    /// Stops the background service. This method is required by <see cref="IHostedService"/>, 
    /// but no cleanup is needed for this initializer.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
