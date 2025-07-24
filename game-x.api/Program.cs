using game_x.api;
using game_x.api.Middleware;
using game_x.application;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.domain.Identity;
using game_x.infrastructure;
using game_x.infrastructure.BackgroundJobs.Scheduling;
using game_x.infrastructure.SignalR.Hubs;
using game_x.persistence;
using game_x.share.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, loggerConfig) =>
{
    loggerConfig
        .Filter.ByExcluding(Matching.WithProperty<string>("RequestPath", p => p.Contains("/healthz")))
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "game-x.api")
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console();
});

// Env
builder.Services.Configure<EngageLabSettings>(builder.Configuration.GetSection("EngageLabSettings"));

// Add services to the container.
builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddSwaggerServices();
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServicesServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthGateMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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

// Middlwware
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<AuditSourceMiddleware>();

// cores
app.UseCors("AllowSpecificOrigin");

// healthz
app.MapHealthChecks("/healthz");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseSerilogRequestLogging();

// SignalR hub
app.MapHub<AdminHub>(AdminHub.Path);
app.MapHub<StoreHub>(StoreHub.Path);
app.MapHub<ClientHub>(ClientHub.Path);

using var scope = app.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;

// Hang fire
HangfireRecurringJobRegistration.RegisterRecurringJobs(serviceProvider);

try
{
    var context = serviceProvider.GetRequiredService<GameXContext>();

    await context.Database.MigrateAsync();

    var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
    var asymmetricCryptoService = serviceProvider.GetRequiredService<IAsymmetricCryptoService>();
    await Seed.SeedData(asymmetricCryptoService, userManager, context);
}
catch (Exception ex)
{
    var logger = serviceProvider.GetRequiredService<IAppLogger<Program>>();
    logger.LogError("An error occurred during migration", ex);
    throw;
}

app.Run();
