using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class AuditLogConfig : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");
        builder.HasKey(al => al.Id);

        builder.Property(al => al.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(al => al.PublicId)
            .HasColumnName("code")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(al => al.EntityName)
            .IsRequired()
            .HasConversion(al => al.Value, al => EntityName.Of(al));

        builder.Property(al => al.EntityId)
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(al => al.Action)
            .IsRequired()
            .HasConversion<short>();

        builder.Property(al => al.ChangedById)
            .IsRequired(false);

        builder.Property(al => al.Source)
            .IsRequired()
            .HasConversion(al => al.Value, al => AuditSource.Of(al));

        builder.Property(al => al.Changes)
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.Property(al => al.SnapshotBefore)
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.Property(al => al.SnapshotAfter)
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.Ignore(al => al.UpdatedAt);

        builder.HasIndex(al => al.PublicId).IsUnique();
    }
}
