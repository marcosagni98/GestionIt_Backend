using API;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

public class Program
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(Program));

    public static void Main(string[] args)
    {
        try
        {
            _log.Info("Starting application");
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            _log.Error($"{ex.Message} for more information {ex.StackTrace}");
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseUrls("https://localhost:5001;http://localhost:5000");
            });
}