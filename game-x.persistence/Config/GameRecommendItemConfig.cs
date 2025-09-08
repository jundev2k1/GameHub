using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public class GameRecommendItemConfig : IEntityTypeConfiguration<GameRecommendItem>
{
    public void Configure(EntityTypeBuilder<GameRecommendItem> builder)
    {
        builder.ToTable("game_recommend_items");

        builder.HasKey(gri => gri.Id);

        builder.Property(gri => gri.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(gri => gri.GameRecommendId)
            .IsRequired();

        builder.Property(gri => gri.GameId)
            .IsRequired();

        builder.Property(gri => gri.Priority)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(gri => gri.CustomTitle)
            .IsRequired(false)
            .HasMaxLength(255);

        builder.Property(gri => gri.IsActive)
            .IsRequired()
            .HasConversion<short>()
            .HasDefaultValue(true);

        builder.HasOne(gri => gri.Game)
            .WithMany()
            .HasForeignKey(gri => gri.GameId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
