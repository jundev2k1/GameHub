using game_x.application.Common.Behaviors;
using game_x.application.Contract.Infrastructure.Services.EmailProcessor;
using game_x.application.Contract.Infrastructure.Services.VerificationCode;
using game_x.application.Services.Notification;
using game_x.application.Services.Verification;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using game_x.application.Features.Rewards.Processors;
using game_x.application.Features.Rewards.Strategies.Missions;

namespace game_x.application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMapster();
        TypeAdapterConfig.GlobalSettings.Scan(AppDomain.CurrentDomain.GetAssemblies());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddMemoryCache();

        services.AddScoped<IVerificationCodeService, MemoryVerificationCodeService>();
        services.AddScoped<IEmailVerificationProcessor, EmailVerificationService>();
        
        services.AddScoped<IMissionProcessor, MissionProcessor>();

        services.AddMissionProgressStrategy();
        return services;
    }
    
    private static IServiceCollection AddMissionProgressStrategy(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<IMissionProgressStrategy>()
            .AddClasses(c => c.AssignableTo<IMissionProgressStrategy>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}