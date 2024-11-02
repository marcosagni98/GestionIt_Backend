using API;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using System.Globalization;

#pragma warning disable CS1591 // Falta el comentario XML para el tipo o miembro visible públicamente
public class Program

{
    private static readonly ILog _log = LogManager.GetLogger(typeof(Program));

    public static void Main(string[] args)
    {
        // Set the environment variable for globalization support
        Environment.SetEnvironmentVariable("DOTNET_SYSTEM_GLOBALIZATION_INVARIANT", "false");

        try
        {
            _log.Info("Starting application");
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

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
                webBuilder.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.ConfigureEndpointDefaults(endpointOptions =>
                    {
                        endpointOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
                    });
                });
                webBuilder.UseUrls("http://0.0.0.0:5000");
            });
}
#pragma warning restore CS1591 // Falta el comentario XML para el tipo o miembro visible públicamente