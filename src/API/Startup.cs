using Domain.Interfaces;
using Infraestructure;
using Infraestructure.Repositories;
using Microsoft.EntityFrameworkCore;


namespace API;

public class Startup
{
    public IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    // Método para configurar los servicios de la aplicación (antes era ConfigureServices)
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite("Data Source=mydatabase.db"));

        services.AddScoped<IIncidentHistoryRepository, IncidentHistoryRepository>();
        services.AddScoped<IIncidentRepository, IncidentRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IUserFeedbackRepository, UserFeedbackRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWorkLogsRepository, WorkLogsRepository>();
    }

    // Método para configurar el pipeline HTTP (antes era Configure)
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
