using Infraestructure;
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
            options.UseSqlite("Data Source=mydatabase.db",
                  b => b.MigrationsAssembly("Infraestructure")));


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
