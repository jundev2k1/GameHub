using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class GameTranslationConfig : IEntityTypeConfiguration<GameTranslation>
{
    public void Configure(EntityTypeBuilder<GameTranslation> builder)
    {
        builder.ToTable("game_translations");

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

        builder.HasIndex(x => new { x.GameId, x.LanguageCode })
            .IsUnique();

        builder.HasOne(x => x.Game)
            .WithMany(g => g.Translations)
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
