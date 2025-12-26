using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Events;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Email;
using game_x.application.Contract.Infrastructure.ExternalApi.GameBaccarat;
using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.ExternalApi.PaymentGateway;
using game_x.application.Contract.Infrastructure.ExternalApi.Srs;
using game_x.application.Contract.Infrastructure.ExternalApi.Uxm;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Jobs;
using game_x.application.Contract.Polly;
using game_x.infrastructure.Auth.Handlers;
using game_x.infrastructure.Caching;
using game_x.infrastructure.Email;
using game_x.infrastructure.Eventing;
using game_x.infrastructure.Extensions;
using game_x.infrastructure.ExternalApi.GameBaccarat;
using game_x.infrastructure.ExternalApi.GameBaccarat.Intercepters;
using game_x.infrastructure.ExternalApi.GameProvider;
using game_x.infrastructure.ExternalApi.GameProvider.Intercepters;
using game_x.infrastructure.ExternalApi.PaymentGateway;
using game_x.infrastructure.ExternalApi.Srs;
using game_x.infrastructure.ExternalApi.Uxm;
using game_x.infrastructure.logger;
using game_x.infrastructure.MediaStorage;
using game_x.infrastructure.Security;
using game_x.infrastructure.Security.Asymmetric;
using game_x.infrastructure.Security.Encryption;
using game_x.infrastructure.Security.HMac;
using game_x.persistence.Requirements;
using game_x.share.Settings;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Minio;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Refit;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using game_x.application.Contract.Infrastructure.ExternalApi.IEtl998;
using game_x.infrastructure.ExternalApi.Etl998;
using game_x.infrastructure.ExternalApi.Etl998.Interceptors;
using game_x.application.Contract.Infrastructure.ExternalApi.SasSlot;
using game_x.infrastructure.ExternalApi.SasSlot;

namespace game_x.infrastructure;

public static class InfrastructureServicesRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSignalR()
            .AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false));
            });
        services.AutoBindSettings(configuration, typeof(BaseSettings).Assembly)
            .AddJwtAuth(configuration)
            .AddHmacAuth()
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
        services.AddScoped(typeof(ISeekCursorBuilder<>), typeof(SeekCursorBuilder<>));

        // Add external services
        services.AddScoped<IEmailService, EngageLabEmailService>();
        services.AddScoped<IUxmService, UxmService>();
        services.AddScoped<IPaymentGatewayService, PaymentGatewayService>();
        services.AddScoped<IGameProviderService, GameProviderService>();
        services.AddScoped<IGameBaccaratService, GameBaccaratService>();
        services.AddScoped<ISasSlotService, SasSlotService>();
        services.AddScoped<ISrsService, SrsService>();
        services.AddScoped<IEtl998Service, Etl998Service>();
        services.AddScoped<IFileStorageService, FileStorageService>();

        // Add security services
        services.AddSingleton<IAsymmetricCryptoService, AsymmetricCryptoService>();
        services.AddSingleton<IGameAesEncryptor, GameAesEncryptor>();
        services.AddSingleton<IAesEncryptor, SystemAesEncryptor>();
        services.AddSingleton<IUserIdProvider, GidQueryUserIdProvider>();

        // Add Hmac service
        services.AddScoped<IHmacNonceStore, HmacNonceStore>();
        services.AddScoped<IHmacValidator, HmacValidator>();

        // Add service DI
        services.Scan(scan => scan.FromApplicationDependencies()
            .AddClasses(c => c.AssignableTo<IServices>().Where(t => !t.IsAbstract))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }

    /// <summary>
    /// Add JWT authentication to the DI container.
    /// </summary>
    private static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(option =>
            {
                var jwtKey = configuration["JwtSettings:Key"]
                    ?? throw new InvalidOperationException("JwtSettings:Key Not configured");

                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
                option.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // SignalR will put the token in the URL query parameter with the parameter name access_token
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        // The connection URL is only checked if it is a Hubs-related path
                        bool hasToken = !string.IsNullOrEmpty(accessToken);
                        bool shouldValidPath = path.StartsWithSegments("/hubs");
                        if (hasToken && shouldValidPath)
                            context.Token = accessToken;

                        return Task.CompletedTask;
                    },
                };
            });

        return services;
    }

    private static IServiceCollection AddHmacAuth(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(AppPolicies.RequireHmac, p => p.Requirements.Add(new HmacRequirement()));

        services.AddScoped<IAuthorizationHandler, HmacAuthorizationHandler>();

        return services;
    }

    private static IServiceCollection AddBackgroundJobs(this IServiceCollection services)
    {
        services.Scan(scan => scan.FromApplicationDependencies()
            .AddClasses(c => c.AssignableTo<IRecurringJob>().Where(t => !t.IsAbstract))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }

    private static IServiceCollection AddHubServices(this IServiceCollection services)
    {
        services.Scan(scan => scan.FromApplicationDependencies()
            .AddClasses(c => c.AssignableTo<IHubServices>().Where(t => !t.IsAbstract))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }

    private static IServiceCollection AddCacheServices(this IServiceCollection services)
    {
        services.Scan(scan => scan.FromApplicationDependencies()
            .AddClasses(c => c.AssignableTo<CacheService>().Where(t => !t.IsAbstract))
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
                    ?? throw new InvalidOperationException("UxmSettings:Host is not configured");
                c.BaseAddress = new Uri(baseUrl);
                c.Timeout = TimeSpan.FromSeconds(5);
            })
            .AddPolicyHandler((sp, _) => sp.GetRequiredService<IHttpPolicyService>().GetRetryPolicy());

        // Payment Gateway API
        services.AddRefitClient<IPaymentGatewayApi>()
            .ConfigureHttpClient(c =>
            {
                var baseUrl = configuration["PaymentGatewaySettings:Host"]
                              ?? throw new InvalidOperationException("PaymentGatewaySettings:Host is not configured");
                c.BaseAddress = new Uri(baseUrl);
                c.Timeout = TimeSpan.FromSeconds(5);
            })
            .AddPolicyHandler((sp, _) => sp.GetRequiredService<IHttpPolicyService>().GetRetryPolicy());
        
        // EngageLab API
        services
            .AddRefitClient<IEngageLabEmailApi>(new RefitSettings
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
            })
            .AddPolicyHandler((sp, _) => sp.GetRequiredService<IHttpPolicyService>().GetRetryPolicy());

        // Game Provider API
        services.AddTransient<CustomApiResponseHandler>();
        services.AddRefitClient<IGameProviderApi>()
            .ConfigureHttpClient(c =>
            {
                var baseUrl = configuration["GameProviderSettings:Host"]
                    ?? throw new InvalidOperationException("GameProviderSettings:Host not configured");
                var apiToken = configuration["GameProviderSettings:ApiToken"]
                    ?? throw new InvalidOperationException("GameProviderSettings:ApiToken not configured");
                c.BaseAddress = new Uri(baseUrl);
                c.Timeout = TimeSpan.FromSeconds(5);
                c.DefaultRequestHeaders.Add("Authorization", apiToken);
            })
            .AddHttpMessageHandler<CustomApiResponseHandler>()
            .AddPolicyHandler((sp, _) => sp.GetRequiredService<IHttpPolicyService>().GetRetryPolicy());

        // Game Baccarat API
        services.AddTransient<HmacMessageHandler>();
        services.AddRefitClient<IGameBaccaratApi>()
            .ConfigureHttpClient(c =>
            {
                var baseUrl = configuration["BaccaratHmacSettings:Host"]
                    ?? throw new InvalidOperationException("BaccaratHmacSettings:Host not configured");
                c.BaseAddress = new Uri(baseUrl);
                c.Timeout = TimeSpan.FromSeconds(5);
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddHttpMessageHandler<HmacMessageHandler>()
            .AddPolicyHandler((sp, _) => sp.GetRequiredService<IHttpPolicyService>().GetRetryPolicy());

        // SAS Slot API
        services.AddRefitClient<ISasSlotApi>()
            .ConfigureHttpClient(c =>
            {
                var baseUrl = configuration["GameSlotSettings:Host"]
                    ?? throw new InvalidOperationException("GameSlotSettings:Host not configured");
                c.BaseAddress = new Uri(baseUrl);
                c.Timeout = TimeSpan.FromSeconds(5);
            })
            .AddPolicyHandler((sp, _) => sp.GetRequiredService<IHttpPolicyService>().GetRetryPolicy());

        // SRS API
        services.AddRefitClient<ISrsApi>()
            .ConfigureHttpClient(c =>
            {
                var baseUrl = configuration["SrsSettings:Host"]
                    ?? throw new InvalidOperationException("SrsSettings:Host not configured");
                c.BaseAddress = new Uri(baseUrl);
                c.Timeout = TimeSpan.FromSeconds(5);
            })
            .AddHttpMessageHandler<CustomApiResponseHandler>()
            .AddPolicyHandler((sp, _) => sp.GetRequiredService<IHttpPolicyService>().GetRetryPolicy());

        // Etl998 API
        services.AddTransient<Etl998Md5MessageHandler>();
        services.AddRefitClient<IEtl998Api>()
            .ConfigureHttpClient(c =>
            {
                var baseUrl = configuration["Etl998Settings:Host"]
                              ?? throw new InvalidOperationException("Etl998Settings:Host is not configured");
                c.BaseAddress = new Uri($"{baseUrl}/GameAPIV4");
                c.Timeout = TimeSpan.FromSeconds(5);
            })
            .AddHttpMessageHandler<Etl998Md5MessageHandler>()
            .AddPolicyHandler((sp, _) => sp.GetRequiredService<IHttpPolicyService>().GetRetryPolicy());
        
        return services;
    }

    private static IServiceCollection AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config =>
        {
            var hangfireConnectionString = configuration.GetConnectionString("HangfireConnection")
                ?? throw new InvalidOperationException("HangfireConnection is not configured");
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