using API.Helpers;
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
/// <param name="configuration"></param>
public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; }  = configuration;


    //Method to add services to the container (before was ConfigureServices)
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
           options.UseSqlite("Data Source=mydatabase.db"));

        services.AddSingleton<ILog>(provider => LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType));

        services.AddControllers();
        services.AddEndpointsApiExplorer();


        services.AddScoped<IIncidentHistoryRepository, IncidentHistoryRepository>();
        services.AddScoped<IIncidentRepository, IncidentRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IUserFeedbackRepository, UserFeedbackRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWorkLogsRepository, WorkLogsRepository>();

        var servicesProvider = services.BuildServiceProvider();
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
        });
        // Add this line to enable CORS
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
        services.AddExceptionHandler<ExceptionHandler>();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Incident Management API", Version = "v1" });
            //TODO: c.AddSecurityDefinition("Bearer", new
            var filePathApiXml = Path.Combine(AppContext.BaseDirectory, "API.xml");
            var filePathApplicationXml = Path.Combine(AppContext.BaseDirectory, "Application.xml");
            c.IncludeXmlComments(filePathApiXml);
            c.IncludeXmlComments(filePathApplicationXml);
        });
    }

    // Method to configure the HTTP request pipeline (before was Configure)
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseExceptionHandler(opt => { });
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Incident Management API"));
        }

        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseCors("AllowAllOrigins"); // Add this line to enable CORS

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Incident Management API"));
    }
}
