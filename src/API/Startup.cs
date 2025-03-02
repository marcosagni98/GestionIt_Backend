using API.Helpers;
using API.Hubs;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;


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


        // CORS setup
        services.AddCors();

        // Exception handling
        services.AddExceptionHandler<ExceptionHandler>();

        // Swagger configuration
        ConfigureSwagger(services);
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