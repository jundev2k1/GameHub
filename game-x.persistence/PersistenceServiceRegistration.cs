using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.persistence.Interceptors;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        services
            .AddIdentityCore<User>(options =>
            {
                // Password policy
                options.Password.RequireDigit = true;             // At least one number is required
                options.Password.RequiredLength = 8;              // The length must be at least 8
                options.Password.RequireNonAlphanumeric = false;  // No special characters required
                options.Password.RequireUppercase = true;         // At least one uppercase letter
                options.Password.RequireLowercase = true;         // At least one lowercase letter

                // User settings
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;

                // Lock settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // Token settings
                options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;

                // Login Settings
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<GameXContext>()
            .AddSignInManager<SignInManager<User>>()
            .AddDefaultTokenProviders();

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