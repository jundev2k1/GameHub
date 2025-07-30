using game_x.persistence.Extensions;
using game_x.persistence.Interceptors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace game_x.persistence;

public sealed class GameXContext(
    DbContextOptions<GameXContext> options,
    IEnumerable<ISaveChangesInterceptor> interceptors)
    : IdentityDbContext<
        User,
        Role,
        string,
        IdentityUserClaim<string>,
        UserRole,
        IdentityUserLogin<string>,
        IdentityRoleClaim<string>,
        IdentityUserToken<string>>(options)
{
    public DbSet<User> AppUsers { get; set; }
    public DbSet<UserRole> AppUserRoles { get; set; }
    public DbSet<UserKyc> UserKycs { get; set; }
    public DbSet<AsymmetricKey> AsymmetricKeys { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<MediaFile> MediaFiles { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(GameXContext).Assembly);
        builder.ApplyAuditColumnsConfiguration();

        // Set table name to snake case for identities
        builder.UseSnakeCaseIdentityTableNames();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (IsAuditSaveInProgress)
            return await base.SaveChangesAsync(cancellationToken);

        // Apply interceptors before saving changes
        foreach (var interceptor in interceptors)
            await interceptor.OnBeforeSaveAsync(this, cancellationToken);

        var result = await base.SaveChangesAsync(cancellationToken);

        // Apply interceptors after saving changes
        IsAuditSaveInProgress = true;
        foreach (var interceptor in interceptors)
            await interceptor.OnAfterSaveAsync(this, cancellationToken);
        IsAuditSaveInProgress = false;

        return result;
    }

    public bool IsAuditSaveInProgress { get; set; } = false;
}
