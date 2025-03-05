using API.Hubs;

/// <summary>
/// Prrovides extension methods for adding application request pipelines to the IApplicationBuilder.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configures the application's request pipeline, including middleware for exception handling,
    /// routing, CORS, authentication, authorization, and integration with Swagger and SignalR.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> instance used to configure the application's request pipeline.</param>
    public static void ConfigureApplication(this WebApplication app)
    {
        // Exception handling middleware
        app.UseExceptionHandler(opt => { });

        // Swagger configuration for development environment
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Incident Management API"));
        }

        // Routing configuration
        app.UseRouting();

        // CORS configuration
        app.UseCors("AllowAllOrigins");

        // Authentication and authorization configuration
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHub<ChatHub>("/messagehub");
    }
}
