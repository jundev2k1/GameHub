using game_x.api;
using game_x.application;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.domain.Entities;
using game_x.infrastructure;
using game_x.infrastructure.BackgroundJobs.Scheduling;
using game_x.persistence;
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

// Add services to the container.
builder.Services
    .AddApiServices(builder.Configuration)
    .AddApplicationServices()
    .AddPersistenceServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseApplicationPipeline();

using var scope = app.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;

// Seed data
try
{
    var context = serviceProvider.GetRequiredService<GameXContext>();
    await context.Database.MigrateAsync();

    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
    var asymmetricCryptoService = serviceProvider.GetRequiredService<IAsymmetricCryptoService>();
    await Seed.SeedData(asymmetricCryptoService, userManager, context);

    var gameProviderCache = serviceProvider.GetRequiredService<IGameProviderCacheService>();
    await gameProviderCache.RefreshGamePlatformList();
    await gameProviderCache.RefreshGameCategoryList();
    await gameProviderCache.RefreshGameTypeList();
    await gameProviderCache.RefreshGameTagList();
    await gameProviderCache.RefreshGameList();

    var refreshTokenManager = serviceProvider.GetRequiredService<IRefreshTokenManagerCacheService>();
    refreshTokenManager.InitRefreshTokens();
}
catch (Exception ex)
{
    var logger = serviceProvider.GetRequiredService<IAppLogger<Program>>();
    logger.LogError("An error occurred during migration", ex);
    throw;
}

// Hang fire
HangfireRecurringJobRegistration.RegisterRecurringJobs(serviceProvider);

app.Run();
