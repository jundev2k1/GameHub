using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class MediaFileConfig : IEntityTypeConfiguration<MediaFile>
{
    public void Configure(EntityTypeBuilder<MediaFile> builder)
    {
        builder.ToTable("media_files");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(m => m.BucketName)
            .IsRequired()
            .HasConversion(m => m.Value, m => BucketName.Of(m));

        builder.Property(m => m.ObjectName)
            .IsRequired()
            .HasConversion(m => m.Value, m => ObjectName.Of(m));

        builder.Property(m => m.FileName)
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(m => m.MimeType)
            .IsRequired()
            .HasConversion(m => m.Value, m => MimeType.Of(m));

        builder.Property(m => m.SizeBytes)
            .HasColumnType("bigint")
            .IsRequired();

        builder.Property(m => m.Metadata)
            .HasColumnType("jsonb")
            .IsRequired(false);
    }
}
