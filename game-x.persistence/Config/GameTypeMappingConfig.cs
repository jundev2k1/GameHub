using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public class GameTypeMappingConfig : IEntityTypeConfiguration<GameTypeMapping>
{
    public void Configure(EntityTypeBuilder<GameTypeMapping> builder)
    {
        builder.ToTable("game_type_mappings");

        builder.HasKey(m => new { m.GameId, m.TypeId });

        builder.Property(m => m.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(m => m.Priority)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Ignore(m => m.Id);

        builder.HasOne(m => m.Game)
            .WithMany(g => g.GameTypeMappings)
            .HasForeignKey(m => m.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Type)
            .WithMany(gt => gt.GameTypeMappings)
            .HasForeignKey(m => m.TypeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
