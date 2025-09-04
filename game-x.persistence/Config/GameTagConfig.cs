using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public class GameTagConfig : IEntityTypeConfiguration<GameTag>
{
    public void Configure(EntityTypeBuilder<GameTag> builder)
    {
        builder.ToTable("game_tags");

        builder.HasKey(gt => gt.Id);

        builder.Property(gt => gt.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(gt => gt.PublicId)
            .HasColumnName("code")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(gt => gt.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasDefaultValue(string.Empty);

        builder.Property(gt => gt.Description)
            .IsRequired()
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty);

        builder.Property(gt => gt.Note)
            .IsRequired()
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty);

        builder.Property(gt => gt.Icon)
            .IsRequired()
            .HasConversion(v => v.Value, v => GameTagIcon.Of(v))
            .HasDefaultValue(string.Empty);

        builder.Property(gt => gt.Color)
            .IsRequired()
            .HasConversion(v => v.Value, v => GameTagColor.Of(v))
            .HasDefaultValue(string.Empty);

        builder.Property(gt => gt.IsActive)
            .IsRequired()
            .HasConversion<short>()
            .HasDefaultValue(true);

        builder.HasIndex(gt => gt.PublicId).IsUnique();
    }
}
