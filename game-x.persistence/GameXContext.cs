using game_x.persistence.Interceptors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace game_x.persistence;

public sealed class GameXContext(
    DbContextOptions<GameXContext> options,
    IEnumerable<ISaveChangesInterceptor> interceptors)
    : IdentityDbContext<
        AppUser,
        IdentityRole,
        string,
        IdentityUserClaim<string>,
        AppUserRole,
        IdentityUserLogin<string>,
        IdentityRoleClaim<string>,
        IdentityUserToken<string>>(options)
{
    public DbSet<AppUser> AppUser { get; set; }
    public DbSet<AppUserRole> AppUserRole { get; set; }
    public DbSet<AsymmetricKey> AsymmetricKey { get; set; }
    public DbSet<BankAccount> BankAccounts { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<MediaFile> MediaFiles { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(GameXContext).Assembly);
        base.OnModelCreating(builder);

        builder.Entity<AppUserRole>(ur =>
        {
            ur.HasKey(k => new { k.UserId, k.RoleId });

            ur.HasOne(r => r.Role)
              .WithMany()
              .HasForeignKey(r => r.RoleId)
              .IsRequired();

            ur.HasOne(r => r.User)
              .WithMany(u => u.UserRoles)
              .HasForeignKey(r => r.UserId)
              .IsRequired();
        });
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
