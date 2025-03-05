using API.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace API.DependencyInjection;

/// <summary>
/// Provides extension methods for adding services to the IServiceCollection.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds presentation layer services to the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configuration">Configuration interface</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogging();
        services.AddAuthenticationJwt(configuration);
        services.AddSwaggerGen();
        services.AddControllers();
        services.AddSignalR();
        services.AddEndpointsApiExplorer();
        services.AddCors();
        services.AddExceptionHandler<ExceptionHandler>();

        return services;
    }

    /// <summary>
    /// Adds CORS services to the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The updated IServiceCollection.</returns>
    private static IServiceCollection AddCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder => builder.AllowAnyOrigin()
                                  .AllowAnyMethod()
                                  .AllowAnyHeader());
        });
        return services;
    }

    /// <summary>
    /// Adds Swagger generation services to the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The updated IServiceCollection.</returns>
    private static IServiceCollection AddSwaggerGen(this IServiceCollection services)
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
        return services;
    }

    /// <summary>
    /// Adds JWT authentication services to the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The updated IServiceCollection.</returns>
    private static IServiceCollection AddAuthenticationJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"])),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                ClockSkew = TimeSpan.Zero
            };
        });
        return services;
    }

    /// <summary>
    /// Adds logging services to the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The updated IServiceCollection.</returns>
    private static IServiceCollection AddLogging(this IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        });
        return services;
    }
}
