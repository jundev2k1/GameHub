using game_x.api.Middleware;
using game_x.infrastructure.SignalR.Hubs;
using Serilog;

namespace game_x.api;

public static class ApplicationPipeline
{
    /// <summary>
    /// Configures the application's middleware pipeline.
    /// </summary>
    public static WebApplication UseApplicationPipeline(this WebApplication app)
    {
        app.UseSwaggerApiDoc();
        app.UseMiddlewares();
        app.UseRouting();
        app.UseCors("AllowSpecificOrigin");
        app.MapHealthChecks("/healthz");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseSerilogRequestLogging();
        app.UseRealtimeHubs();

        return app;
    }

    /// <summary>
    /// Enables Swagger UI in Development.
    /// </summary>
    private static WebApplication UseSwaggerApiDoc(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Game X");
                options.RoutePrefix = string.Empty;
            });
        }
        return app;
    }

    /// <summary>
    /// Adds custom middlewares.
    /// </summary>
    private static WebApplication UseMiddlewares(this WebApplication app)
    {
        //app.UseMiddleware<ExceptionMiddleware>();
        app.UseMiddleware<AuditSourceMiddleware>();
        return app;
    }

    /// <summary>
    /// Maps SignalR hubs.
    /// </summary>
    private static WebApplication UseRealtimeHubs(this WebApplication app)
    {
        app.MapHub<AdminHub>(AdminHub.Path);
        app.MapHub<CsAdminHub>(CsAdminHub.Path);
        app.MapHub<ClientHub>(ClientHub.Path);
        app.MapHub<ChatHub>(ChatHub.Path);
        app.MapHub<LiveStreamHub>(LiveStreamHub.Path);
        return app;
    }
}
