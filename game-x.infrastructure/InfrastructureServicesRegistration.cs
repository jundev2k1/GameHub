using game_x.application.Common.Abstractions.Events;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Email;
using game_x.application.Contract.Infrastructure.ExternalApi.Uxm;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Polly;
using game_x.infrastructure.Email;
using game_x.infrastructure.Eventing;
using game_x.infrastructure.Extensions;
using game_x.infrastructure.ExternalApi.Uxm;
using game_x.infrastructure.Identity;
using game_x.infrastructure.logger;
using game_x.infrastructure.MediaStorage;
using game_x.infrastructure.Polly;
using game_x.infrastructure.Security;
using game_x.infrastructure.Security.Asymmetric;
using game_x.infrastructure.SignalR.Services;
using game_x.share.Settings;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Refit;
using System.Net.Http.Headers;
using System.Text;

namespace game_x.infrastructure;

public static class InfrastructureServicesRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSignalR();
        services.AutoBindSettings(configuration, typeof(BaseSettings).Assembly)
            .AddHangfireServices(configuration)
            .AddExternalApiServices(configuration)
            .AddMinioServices(configuration)
            .AddCacheServices()
            .AddBackgroundJobs()
            .AddHubServices()
            .AddDependencyInjections();

        return services;
    }

    private static IServiceCollection AddDependencyInjections(this IServiceCollection services)
    {
        // Logging
        services.AddSingleton(typeof(IAppLogger<>), typeof(LoggerAdapter<>));

        // Eventing
        services.AddScoped<IApplicationEventDispatcher, ApplicationEventDispatcher>();

        // Builders
        services.AddScoped(typeof(ICriteriaBuilder<>), typeof(CriteriaBuilder<>));

        // Services
        services.AddScoped<IUserAccessor, UserAccessor>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAsymmetricCryptoService, AsymmetricCryptoService>();
        services.AddScoped<IUxmService, UxmService>();
        services.AddScoped<IEmailService, EngageLabEmailService>();
        services.AddScoped<IHttpPolicyService, HttpPolicyService>();
        services.AddScoped<IFileStorageService, FileStorageService>();

        return services;
    }

    private static IServiceCollection AddBackgroundJobs(this IServiceCollection services)
    {
        return services;
    }

    private static IServiceCollection AddHubServices(this IServiceCollection services)
    {
        services.AddSingleton<IClientHubService, ClientHubService>();
        services.AddSingleton<IAdminHubService, AdminHubService>();

        return services;
    }

    private static IServiceCollection AddCacheServices(this IServiceCollection services)
    {
        services.Scan(scan => scan.FromApplicationDependencies()
            .AddClasses(c => c.Where(t => t.Name.EndsWith("CacheService")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        return services;
    }

    private static IServiceCollection AddExternalApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        // UXM API
        services.AddRefitClient<IUxmApi>()
            .ConfigureHttpClient(c =>
            {
                var baseUrl = configuration["UxmSettings:Host"]
                    ?? throw new InvalidOperationException("UxmSettings:Host 未設定");
                c.BaseAddress = new Uri(baseUrl);
                c.Timeout = TimeSpan.FromSeconds(5);
            })
            .AddPolicyHandler((sp, _) => sp.GetRequiredService<IHttpPolicyService>().GetRetryPolicy());

        // EngageLab API
        services.AddRefitClient<IEngageLabEmailApi>(new RefitSettings
        {
            ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                NullValueHandling = NullValueHandling.Ignore
            })
        })
            .ConfigureHttpClient(c =>
            {
                var baseUrl = configuration["EngageLabSettings:BaseUrl"]
                    ?? throw new InvalidOperationException("EngageLabSettings:BaseUrl not configured");
                var apiUser = configuration["EngageLabSettings:ApiUser"]
                    ?? throw new InvalidOperationException("EngageLabSettings:ApiUser not configured");
                var apiKey = configuration["EngageLabSettings:ApiKey"]
                    ?? throw new InvalidOperationException("EngageLabSettings:ApiKey not configured");
                var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiUser}:{apiKey}"));

                c.BaseAddress = new Uri(baseUrl);
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                c.Timeout = TimeSpan.FromSeconds(10);
            }).AddPolicyHandler((sp, _) => sp.GetRequiredService<IHttpPolicyService>().GetRetryPolicy());

        return services;
    }

    private static IServiceCollection AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config =>
        {
            var hangfireConnectionString = configuration.GetConnectionString("HangfireConnection")
                ?? throw new InvalidOperationException("HangfireConnection 未設定");
            var options = new PostgreSqlStorageOptions
            {
                SchemaName = "GameXHangfire",
                PrepareSchemaIfNecessary = true,
                QueuePollInterval = TimeSpan.FromSeconds(5),
                InvisibilityTimeout = TimeSpan.FromMinutes(10),
                UseSlidingInvisibilityTimeout = true,
                DistributedLockTimeout = TimeSpan.FromMinutes(5),
                JobExpirationCheckInterval = TimeSpan.FromMinutes(30),
                CountersAggregateInterval = TimeSpan.FromMinutes(5),
                DeleteExpiredBatchSize = 1000,
                EnableLongPolling = true,
                UseNativeDatabaseTransactions = true,
                EnableTransactionScopeEnlistment = true,
                AllowUnsafeValues = false
            };
            config.UsePostgreSqlStorage(
                bootstrapperOptions => bootstrapperOptions.UseNpgsqlConnection(hangfireConnectionString), options);
        });
        services.AddHangfireServer();

        return services;
    }

    private static IServiceCollection AddMinioServices(this IServiceCollection services, IConfiguration configuration)
     {
         services.AddMinio(options =>
         {
             var endpoint = configuration["MinioSettings:InternalEndpoint"]
                 ?? throw new InvalidOperationException("MinioSettings:InternalEndpoint is not configured");
             var accessKey = configuration["MinioSettings:AccessKey"]
                 ?? throw new InvalidOperationException("MinioSettings:AccessKey is not configured");
             var secretKey = configuration["MinioSettings:SecretKey"]
                 ?? throw new InvalidOperationException("MinioSettings:SecretKey is not configured");
 
             options.WithEndpoint(endpoint);
             options.WithCredentials(accessKey, secretKey);
             options.WithSSL(false);
             options.Build();
         });
         return services;
     }
}
