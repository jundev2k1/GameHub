using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class StaffCounterConfig : IEntityTypeConfiguration<StaffCounter>
{
    public void Configure(EntityTypeBuilder<StaffCounter> builder)
    {
        builder.ToTable("staff_counter");
        builder.HasKey(sc => new { sc.Id, sc.UserId, sc.CounterId });

        builder.Property(o => o.Id)
            .HasColumnName("id")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(sc => sc.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(sc => sc.CounterId)
            .HasColumnName("counter_id")
            .IsRequired();

        builder.Property(sc => sc.LoginAt)
            .HasColumnName("login_at")
            .IsRequired();

        builder.Property(sc => sc.LogoutAt)
            .HasColumnName("logout_at")
            .IsRequired(false);

        builder.Property(sc => sc.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(sc => sc.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasOne(sc => sc.Counter)
            .WithMany()
            .HasForeignKey(sc => sc.CounterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sc => sc.User)
            .WithMany()
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
