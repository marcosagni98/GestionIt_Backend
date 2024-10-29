using API.Helpers;
using Application.Helpers.Mappers;
using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Infraestructure;
using Infraestructure.Repositories;
using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;

namespace API;

/// <summary>
/// Class to configure the application's services and request pipeline
/// </summary>
public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    // Method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        // Build the connection string from environment variables
        var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
        var dbName = Environment.GetEnvironmentVariable("DB_NAME");
        var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
        var connectionString = $"Data Source={dbHost}; Initial Catalog={dbName};User ID=sa;Password={dbPassword};TrustServerCertificate=True;";

        // Configure DbContext with SQL Server
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Configure logging
        services.AddSingleton<ILog>(provider => LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType));

        // Add services
        services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

        services.AddEndpointsApiExplorer();


        RegisterAutomapper(services);
        RegisterRepositories(services);
        RegisterServices(services);
        

        // CORS setup
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder => builder.AllowAnyOrigin()
                                  .AllowAnyMethod()
                                  .AllowAnyHeader());
        });

        // Exception handling
        services.AddExceptionHandler<ExceptionHandler>();

        // Swagger configuration
        ConfigureSwagger(services);
    }

    private void RegisterAutomapper(IServiceCollection services)
    {
        services.AddAutoMapper(typeof(Startup));
        services.AddAutoMapper(typeof(UserMapper));
        services.AddAutoMapper(typeof(IncidentHistoryMapper));
        services.AddAutoMapper(typeof(IncidentMapper));
        services.AddAutoMapper(typeof(UserFeedbackMapper));
        services.AddAutoMapper(typeof(MessageMapper));
        services.AddAutoMapper(typeof(WorkLogMapper));
    }

    private void RegisterRepositories(IServiceCollection services)
    {
        services.AddScoped<IIncidentHistoryRepository, IncidentHistoryRepository>();
        services.AddScoped<IIncidentRepository, IncidentRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IUserFeedbackRepository, UserFeedbackRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWorkLogRepository, WorkLogsRepository>();
    }

    private void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IIncidentHistoryService, IncidentHistoryService>();
        services.AddScoped<IIncidentService, IncidentService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IUserFeedbackService, UserFeedbackService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IWorkLogService, WorkLogService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private void ConfigureSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Incident Management API", Version = "v1" });
            var filePathApiXml = Path.Combine(AppContext.BaseDirectory, "API.xml");
            var filePathApplicationXml = Path.Combine(AppContext.BaseDirectory, "Application.xml");
            c.IncludeXmlComments(filePathApiXml);
            c.IncludeXmlComments(filePathApplicationXml);
        });
    }

    // Method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Middleware for exception handling
        app.UseExceptionHandler(opt => { });

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Incident Management API"));
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors("AllowAllOrigins");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
