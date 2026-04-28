using game_x.api.Common.Filters;
using game_x.api.Middleware;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace game_x.api;

public static class ApiServicesRegistration
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddHealthChecks();

        var originsString = config["CorsSettings:AllowedOrigins"];
        var corsOrigins = originsString?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (corsOrigins is null || corsOrigins.Length == 0)
            throw new InvalidOperationException("CorsSettings:AllowedOrigins 環境變數未設定或格式錯誤");

        services.AddCors(options =>
        {
            options.AddPolicy(
                "AllowSpecificOrigin",
                builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithOrigins(corsOrigins));
        });

        services.AddHttpContextAccessor();
        services.AddEndpointsApiExplorer();
        services.AddDataProtection();
        services.AddSwaggerServices();

        // Add controllers,
        // Configure to automatically convert an Enum int type to a string type for API request/response
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false)
            );
        });

        services.AddOpenApi();
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthGateMiddleware>();

        return services;
    }

    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            // Add custom header
            c.OperationFilter<SwaggerHeaderFilter>();

            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Game-X API",
                Version = "v1",
                Description = "API documentation for Game-X application",
                Contact = new OpenApiContact
                {
                    Name = "Support Team",
                    Email = "support@gamex.com"
                }
            });

            // Add JWT Authentication to Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter JWT with Bearer into field",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
