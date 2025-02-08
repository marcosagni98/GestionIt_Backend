using API.Helpers;
using API.Hubs;
using Application.Helpers.Mappers;
using Application.Interfaces.Services;
using Application.Interfaces.Utils;
using Application.Services;
using Application.Utils;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Utils;
using Infrastructure;
using Infrastructure.Repositories;
using Infrastructure.Utils;
using log4net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace API;

/// <summary>
/// Class to configure the application's services and request pipeline
/// </summary>
public class Startup
{
#pragma warning disable CS1591 
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
        var dbUser = Environment.GetEnvironmentVariable("DB_USER");
        var connectionString = $"Data Source={dbHost}; Initial Catalog={dbName};User ID={dbUser};Password={dbPassword};TrustServerCertificate=True;";

        // Configure DbContext with SQL Server
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Configure logging
        services.AddSingleton<ILog>(provider => LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType));

        var key = Encoding.ASCII.GetBytes("supersecretkeysupersecretkeysupersecretkey");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = "gestionIt_api",
                ValidAudience = "gestionIt_frontend",
                ClockSkew = TimeSpan.Zero
            };
        });

        // Add services
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
            });

        services.AddSignalR();

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
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddTransient<IEmailSender,EmailSender>();
    }

    private void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IIncidentHistoryService, IncidentHistoryService>();
        services.AddScoped<IIncidentService, IncidentService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IUserFeedbackService, UserFeedbackService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IWorkLogService, WorkLogService>();
        services.AddScoped<IStatisticsService, StatisticsService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddHttpClient<IOllamaService, OllamaService>();
        services.AddScoped<IJwt, Jwt>();
    }

    private void ConfigureSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Incident Management API", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' followed by a space and then the JWT token. Example: 'Bearer abcdef12345'"
            });

            c.OperationFilter<AuthorizeCheckOperationFilter>();

            var filePathApiXml = Path.Combine(AppContext.BaseDirectory, "API.xml");
            var filePathApplicationXml = Path.Combine(AppContext.BaseDirectory, "Application.xml");
            c.IncludeXmlComments(filePathApiXml);
            c.IncludeXmlComments(filePathApplicationXml);
        });
    }

    // Method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
    {
        // Middleware for exception handling
        app.UseExceptionHandler(opt => { });

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Incident Management API"));
            
        }
        app.UseRouting();
        app.UseCors("AllowAllOrigins");

        app.UseAuthentication();
        app.UseAuthorization();


        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<ChatHub>("/messagehub");
        });

        InitializeDatabaseAndAdminUser(serviceProvider);
    }

    /// <summary>
    /// Initializes the database and creates an admin user if it doesn't exist.
    /// </summary>
    /// <param name="serviceProvider"></param>
    private void InitializeDatabaseAndAdminUser(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var databaseCreator = context.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            if (databaseCreator != null)
            {
                if (!databaseCreator.CanConnect())
                    databaseCreator.Create();
                if (!databaseCreator.HasTables())
                    databaseCreator.CreateTables();
            }
        }
    }
}
#pragma warning restore CS1591 