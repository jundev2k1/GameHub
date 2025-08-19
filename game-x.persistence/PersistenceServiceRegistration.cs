using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.persistence.Interceptors;
using game_x.persistence.Repo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace game_x.persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services
            .AddDbContext<GameXContext>(options => options
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention())
            .AddJwtAuth(configuration)
            .AddIdentity()
            .AddRepos()
            .AddInterceptors();

        // Register services to the DI container.
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        return services;
    }

    /// <summary>
    /// Add Identity services to the DI container.
    /// </summary>
    private static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<GameXContext>()
            .AddDefaultTokenProviders();

        // Configure Identity options
        services.Configure<IdentityOptions>(options =>
        {
            // Password policy
            options.Password.RequireDigit = true;             // At least one number is required
            options.Password.RequiredLength = 8;              // The length must be at least 8
            options.Password.RequireNonAlphanumeric = false;  // No special characters required
            options.Password.RequireUppercase = true;         // At least one uppercase letter
            options.Password.RequireLowercase = true;         // At least one lowercase letter

            // User settings
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;

            // Lock settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30); // Lock Time (30 minutes)
            options.Lockout.MaxFailedAccessAttempts = 5;                      // Locked after the fifth incorrect password
            options.Lockout.AllowedForNewUsers = true;                        // Can new users be locked?

            // Token settings
            options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;

            // Login Settings
            options.SignIn.RequireConfirmedEmail = false;                     // Email confirmation is not required
            options.SignIn.RequireConfirmedPhoneNumber = false;               // Phone number confirmation is not required
            options.User.RequireUniqueEmail = false;                          // Email uniqueness is not required
        });

        return services;
    }

    /// <summary>
    /// Add JWT authentication to the DI container.
    /// </summary>
    private static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option =>
        {
            var jwtKey = configuration["JwtSettings:Key"] ?? throw new InvalidOperationException("JwtSettings:Key 未設定");
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
                    if (hasToken && shouldValidPath) context.Token = accessToken;
                    return Task.CompletedTask;
                },
            };
        });

        return services;
    }

    /// <summary>
    /// Add repositories to the DI container.
    /// </summary>
    private static IServiceCollection AddRepos(this IServiceCollection services)
    {
        services.Scan(scan => scan.FromApplicationDependencies()
            .AddClasses(c => c.AssignableTo<IRepository>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        
        services.AddScoped<IGameTransactionRepo, GameTransactionRepo>();
        return services;
    }

    /// <summary>
    /// Add interceptors to the DI container.
    /// </summary>
    private static IServiceCollection AddInterceptors(this IServiceCollection services)
    {
        services.AddScoped<ISaveChangesInterceptor, AuditLogInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, TimestampInterceptor>();
        return services;
    }
}
