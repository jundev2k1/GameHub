using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public class GameTagMappingConfig : IEntityTypeConfiguration<GameTagMapping>
{
    public void Configure(EntityTypeBuilder<GameTagMapping> builder)
    {
        builder.ToTable("game_tag_mappings");

        builder.HasKey(m => new { m.GameId, m.TagId });

        builder.Property(m => m.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(m => m.Priority)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Ignore(m => m.Id);

        builder.HasOne(m => m.Game)
            .WithMany(g => g.GameTagMappings)
            .HasForeignKey(m => m.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Tag)
            .WithMany(gt => gt.GameTagMappings)
            .HasForeignKey(m => m.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
