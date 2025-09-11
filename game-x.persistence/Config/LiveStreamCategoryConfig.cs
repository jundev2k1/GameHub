using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class LiveStreamCategoryConfig : IEntityTypeConfiguration<LiveStreamCategory>
{
    public void Configure(EntityTypeBuilder<LiveStreamCategory> builder)
    {
        builder.ToTable("livestream_categories");

        builder.HasKey(lsc => lsc.Id);

        builder.Property(lsc => lsc.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(lsc => lsc.PublicId)
            .HasColumnName("code")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(lsc => lsc.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasDefaultValue(string.Empty);

        builder.Property(lsc => lsc.Description)
            .IsRequired()
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty);

        builder.Property(lsc => lsc.Note)
            .IsRequired()
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty);

        builder.Property(lsc => lsc.Priority)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(lsc => lsc.IsActive)
            .IsRequired()
            .HasConversion<short>()
            .HasDefaultValue(true);

        builder.HasIndex(lsc => lsc.PublicId).IsUnique();
    }
}
