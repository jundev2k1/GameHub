using game_x.api;
using game_x.application;
using game_x.application.Common.Abstractions.Events;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Events.OnUserBalanceUpdated;
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

if (app.Environment.IsDevelopment())
{
    app.MapGet("/dev/{userId}/balance-changed", async (
        [FromRoute] string userId,
        [AsParameters] BalanceChangedDevInput input,
        [FromServices] IApplicationEventDispatcher eventDispatcher) =>
    {
        var @event = new OnUserBalanceUpdatedEvent(userId, input.PlatformId);
        await eventDispatcher.Publish(@event);
    });
}

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
    await gameProviderCache.RefreshGameRecommendList();
    await gameProviderCache.RefreshGameList();

    var refreshTokenManager = serviceProvider.GetRequiredService<IRefreshTokenManagerCacheService>();
    refreshTokenManager.InitRefreshTokens();

    var liveStreamManager = serviceProvider.GetRequiredService<ILiveStreamManagerCacheService>();
    await liveStreamManager.RefreshGiftCacheAsync();

    var appSettingCache = serviceProvider.GetRequiredService<IAppSettingCacheService>();
    appSettingCache.RefreshCache();
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

public record BalanceChangedDevInput(Guid? PlatformId);