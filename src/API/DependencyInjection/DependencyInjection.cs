using API.Helpers;
using Microsoft.OpenApi.Models;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace API.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddLogging();
        services.AddAuthenticationJwt();
        services.AddControllers();
        services.AddSignalR();
        services.AddEndpointsApiExplorer();
        services.AddCors();
        services.AddExceptionHandler<ExceptionHandler>();
        services.AddSwaggerGen();
        return services;
    }


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

    private static IServiceCollection AddAuthenticationJwt(this IServiceCollection services)
    {
        var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("API_KEY"));
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
        return services;
    }

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
