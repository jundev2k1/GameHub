using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class AppUserRoleConfig : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles");
        builder.HasKey(k => new { k.UserId, k.RoleId });

        builder.HasOne(r => r.Role)
          .WithMany()
          .HasForeignKey(r => r.RoleId)
          .IsRequired();

        builder.HasOne(r => r.User)
          .WithMany(u => u.UserRoles)
          .HasForeignKey(r => r.UserId)
          .IsRequired();
    }
}
