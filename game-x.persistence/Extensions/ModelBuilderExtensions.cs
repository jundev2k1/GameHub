using Microsoft.AspNetCore.Identity;

namespace game_x.persistence.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyAuditColumnsConfiguration(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.ClrType.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntity)))
            {
                var createdAtProperty = entityType.FindProperty(nameof(IEntity.CreatedAt));
                if (createdAtProperty != null)
                {
                    createdAtProperty.SetColumnName("created_at");
                    createdAtProperty.SetColumnType("timestamp with time zone");
                    createdAtProperty.IsNullable = false;
                }

                var updatedAtProperty = entityType.FindProperty(nameof(IEntity.UpdatedAt));
                if (updatedAtProperty != null)
                {
                    updatedAtProperty.SetColumnName("updated_at");
                    updatedAtProperty.SetColumnType("timestamp with time zone");
                    updatedAtProperty.IsNullable = false;
                }
            }
        }
    }

    /// <summary>Set table name to snake case for identities.</summary>
    public static void UseSnakeCaseIdentityTableNames(this ModelBuilder builder)
    {
        builder.Entity<IdentityUserClaim<string>>(b => b.ToTable("user_claims"));
        builder.Entity<IdentityUserLogin<string>>(b => b.ToTable("user_logins"));
        builder.Entity<IdentityUserToken<string>>(b => b.ToTable("user_tokens"));
        builder.Entity<Role>(b => b.ToTable("roles"));
        builder.Entity<IdentityRoleClaim<string>>(b => b.ToTable("role_claims"));
    }
    
    public static void IgnoreRemovedRecords(this ModelBuilder builder)
    {
        builder.Entity<Message>().HasQueryFilter(x => x.IsDeleted == false);
    }
}