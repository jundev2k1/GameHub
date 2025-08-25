using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public class GameCategoryMappingConfig : IEntityTypeConfiguration<GameCategoryMapping>
{
    public void Configure(EntityTypeBuilder<GameCategoryMapping> builder)
    {
        builder.ToTable("game_category_mappings");

        builder.HasKey(m => new { m.GameId, m.CategoryId });

        builder.Property(m => m.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(m => m.Priority)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Ignore(m => m.Id);

        builder.HasOne(m => m.Game)
            .WithMany(g => g.GameCategoryMappings)
            .HasForeignKey(m => m.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Category)
            .WithMany(gc => gc.GameCategoryMappings)
            .HasForeignKey(m => m.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
