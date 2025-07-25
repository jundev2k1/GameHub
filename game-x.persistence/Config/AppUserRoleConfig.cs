using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class AppUserRoleConfig : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles");

        builder.HasKey(r => new { r.UserId, r.RoleId });

        builder.HasOne(r => r.Role)
          .WithMany(ur => ur.UserRoles)
          .HasForeignKey(r => r.RoleId)
          .IsRequired()
          .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.User)
          .WithMany(u => u.UserRoles)
          .HasForeignKey(r => r.UserId)
          .IsRequired()
          .OnDelete(DeleteBehavior.Cascade);
    }
}
