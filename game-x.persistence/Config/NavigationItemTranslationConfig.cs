using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class NavigationItemTranslationConfig : IEntityTypeConfiguration<NavigationItemTranslation>
{
    public void Configure(EntityTypeBuilder<NavigationItemTranslation> builder)
    {
        builder.ToTable("navigation_item_translations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.LanguageCode)
            .IsRequired()
            .HasMaxLength(10)
            .HasConversion(vo => vo.Value, v => LanguageCode.Of(v));

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(x => new { x.NavigationItemId, x.LanguageCode })
            .IsUnique();

        builder.HasOne(x => x.NavigationItem)
            .WithMany(g => g.Translations)
            .HasForeignKey(x => x.NavigationItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
