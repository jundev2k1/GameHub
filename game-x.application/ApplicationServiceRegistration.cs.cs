using game_x.application.Common.Behaviors;
using game_x.application.Contract.Infrastructure.Services.EmailProcessor;
using game_x.application.Contract.Infrastructure.Services.VerificationCode;
using game_x.application.Mappers.Order;
using game_x.application.Services.Notification;
using game_x.application.Services.Verification;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace game_x.application;
public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Mapster
        services.AddMapster();
        TypeAdapterConfig.GlobalSettings.Scan(AppDomain.CurrentDomain.GetAssemblies());

        // 自動註冊所有自定義的 Mapster mapper
        services.Scan(scan => scan.FromAssemblyOf<OrderMapper>()
        .AddClasses(c => c.Where(type => type.Name.EndsWith("Mapper")))
        .AsSelf()
        .WithScopedLifetime());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddMemoryCache();
    
        services.AddScoped<IVerificationCodeService, MemoryVerificationCodeService>();
        services.AddScoped<IEmailVerificationProcessor, EmailVerificationService>();
        
        return services;
    }
}
