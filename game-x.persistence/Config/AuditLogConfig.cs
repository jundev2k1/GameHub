using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class AuditLogConfig : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_log");
        builder.HasKey(al => al.Id);

        builder.Property(al => al.Id)
            .HasColumnName("id")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(al => al.PublicId)
            .HasColumnName("code")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(al => al.EntityName)
            .HasColumnName("entity_name")
            .IsRequired()
            .HasConversion(al => al.Value, al => EntityName.Of(al));

        builder.Property(al => al.EntityId)
            .HasColumnName("entity_id")
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(al => al.Action)
            .HasColumnName("action")
            .IsRequired()
            .HasConversion<short>();

        builder.Property(al => al.ChangedByUserId)
            .HasColumnName("changed_by_user_id")
            .IsRequired(false);

        builder.Property(al => al.Source)
            .HasColumnName("source")
            .IsRequired()
            .HasConversion(al => al.Value, al => AuditSource.Of(al));

        builder.Property(al => al.Changes)
            .HasColumnName("changes")
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.Property(al => al.SnapshotBefore)
            .HasColumnName("snapshot_before")
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.Property(al => al.SnapshotAfter)
            .HasColumnName("snapshot_after")
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.Property(al => al.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Ignore(al => al.UpdatedAt);

        builder.HasOne(al => al.ChangedBy)
            .WithMany()
            .HasForeignKey(al => al.ChangedByUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
