using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public class AppUserConfig : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasOne(u => u.Passport)
            .WithOne(p => p.AppUser)
            .HasForeignKey<UserPassport>(p => p.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(u => u.StaffUser)
            .WithOne(su => su.User)
            .HasForeignKey<StaffUser>(su => su.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(u => u.StaffUsers)
            .WithOne(su => su.Staff)
            .HasForeignKey(su => su.StaffId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
    }
}