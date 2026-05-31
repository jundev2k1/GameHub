using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class GamePlatformTranslationConfig : IEntityTypeConfiguration<GamePlatformTranslation>
{
    public void Configure(EntityTypeBuilder<GamePlatformTranslation> builder)
    {
        builder.ToTable("game_platform_translations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.LanguageCode)
            .IsRequired()
            .HasMaxLength(10)
            .HasConversion(vo => vo.Value, v => LanguageCode.Of(v));

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty);

        builder.Property(x => x.Note)
            .IsRequired()
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty);

        builder.HasIndex(x => new { x.PlatformId, x.LanguageCode })
            .IsUnique();

        builder.HasOne(x => x.Platform)
            .WithMany(g => g.Translations)
            .HasForeignKey(x => x.PlatformId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
