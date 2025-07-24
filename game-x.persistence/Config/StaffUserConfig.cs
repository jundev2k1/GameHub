using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class StaffUserConfig : IEntityTypeConfiguration<StaffUser>
{
    public void Configure(EntityTypeBuilder<StaffUser> builder)
    {
        builder.ToTable("staff_user");
        builder.HasKey(su => new { su.Id, su.CounterId, su.UserId, su.StaffId });

        builder.Property(su => su.Id)
            .HasColumnName("id")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(su => su.CounterId)
            .HasColumnName("counter_id")
            .IsRequired();

        builder.Property(su => su.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(su => su.StaffId)
            .HasColumnName("staff_id")
            .IsRequired();

        builder.Property(su => su.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(su => su.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasOne(su => su.Counter)
            .WithMany()
            .HasForeignKey(su => su.CounterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(su => su.Staff)
            .WithMany()
            .HasForeignKey(su => su.StaffId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(su => su.User)
            .WithOne(u => u.StaffUser)
            .HasForeignKey<StaffUser>(su => su.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(su => su.Staff)
            .WithMany(u => u.StaffUsers)
            .HasForeignKey(su => su.StaffId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
