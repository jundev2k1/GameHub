using game_x.domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class NotificationConfig : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notification");
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .HasColumnName("notification_id")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(n => n.PublicId)
            .HasColumnName("notification_code")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(n => n.MessageKey)
            .HasColumnName("message_key")
            .IsRequired()
            .HasColumnType("smallint")
            .HasConversion<short>();

        builder.Property(n => n.UserId)
            .HasColumnName("user_id")
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(n => n.Type)
            .HasColumnName("type")
            .IsRequired()
            .HasConversion<short>()
            .HasColumnType("smallint")
            .HasDefaultValue(NotificationType.Info);

        builder.Property(n => n.IsRead)
            .HasColumnName("is_read")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(n => n.Severity)
            .HasColumnName("severity")
            .IsRequired()
            .HasConversion<short>()
            .HasColumnType("smallint")
            .HasDefaultValue(NotificationSeverity.Info);

        builder.Property(n => n.ReadAt)
            .HasColumnName("read_at")
            .IsRequired(false);

        builder.Property(n => n.ExpiredAt)
            .HasColumnName("expired_at")
            .IsRequired(false);

        builder.Property(n => n.Metadata)
            .HasColumnName("metadata")
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.Property(n => n.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(n => n.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasIndex(n => n.PublicId).IsUnique();
        builder.HasIndex(n => n.UserId);
        builder.HasIndex(n => n.IsRead);
        builder.HasIndex(n => n.Type);
        builder.HasIndex(n => n.Severity);
        builder.HasIndex(n => n.ExpiredAt);
    }
}
