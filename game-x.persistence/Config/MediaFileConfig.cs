using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class MediaFileConfig : IEntityTypeConfiguration<MediaFile>
{
    public void Configure(EntityTypeBuilder<MediaFile> builder)
    {
        builder.ToTable("media_file");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasColumnName("id")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(m => m.BucketName)
            .HasColumnName("bucket_name")
            .IsRequired()
            .HasConversion(m => m.Value, m => BucketName.Of(m));

        builder.Property(m => m.ObjectName)
            .HasColumnName("object_name")
            .IsRequired()
            .HasConversion(m => m.Value, m => ObjectName.Of(m));

        builder.Property(m => m.FileName)
            .HasColumnName("file_name")
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(m => m.MimeType)
            .HasColumnName("mime_type")
            .IsRequired()
            .HasConversion(m => m.Value, m => MimeType.Of(m));

        builder.Property(m => m.SizeBytes)
            .HasColumnName("size_bytes")
            .HasColumnType("bigint")
            .IsRequired();

        builder.Property(m => m.Metadata)
            .HasColumnName("metadata")
            .IsRequired(false);

        builder.Property(m => m.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(m => m.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
    }
}
