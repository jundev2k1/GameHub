using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class NotificationConfig : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(n => n.PublicId)
            .HasColumnName("code")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(n => n.MessageKey)
            .IsRequired()
            .HasColumnType("smallint")
            .HasConversion<short>();

        builder.Property(n => n.UserId)
            .HasColumnName("user_id")
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(n => n.Type)
            .IsRequired()
            .HasConversion<short>()
            .HasColumnType("smallint")
            .HasDefaultValue(NotificationType.Info);

        builder.Property(n => n.IsRead)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(n => n.Severity)
            .IsRequired()
            .HasConversion<short>()
            .HasColumnType("smallint")
            .HasDefaultValue(NotificationSeverity.Info);

        builder.Property(n => n.ReadAt)
            .IsRequired(false);

        builder.Property(n => n.ExpiredAt)
            .IsRequired(false);

        builder.Property(n => n.Metadata)
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.HasIndex(n => n.PublicId).IsUnique();
        builder.HasIndex(n => n.UserId);
        builder.HasIndex(n => n.IsRead);
        builder.HasIndex(n => n.Type);
        builder.HasIndex(n => n.Severity);
        builder.HasIndex(n => n.ExpiredAt);
    }
}
