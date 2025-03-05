using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace API.Extensions;

/// <summary>
/// Provides extension methods for adding the WebApplicationBuilder.
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Configures Kestrel server options.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder to configure.</param>
    public static void ConfigureKestrel(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.ConfigureEndpointDefaults(endpointOptions =>
            {
                endpointOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
            });
        });
    }

    /// <summary>
    /// Configures application URLs.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder to configure.</param>
    public static void ConfigureUrls(this WebApplicationBuilder builder)
    {
        builder.WebHost.UseUrls("http://0.0.0.0:5000");
    }
}
