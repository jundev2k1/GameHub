using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class LiveStreamCategoryMappingConfig : IEntityTypeConfiguration<LiveStreamCategoryMapping>
{
    public void Configure(EntityTypeBuilder<LiveStreamCategoryMapping> builder)
    {
        builder.ToTable("livestream_category_mappings");

        builder.HasKey(m => new { m.ScheduleId, m.CategoryId });

        builder.Property(m => m.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(m => m.Priority)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Ignore(m => m.Id);

        builder.HasOne(m => m.Schedule)
            .WithMany(ls => ls.CategoryMappings)
            .HasForeignKey(m => m.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Category)
            .WithMany(lsc => lsc.CategoryMappings)
            .HasForeignKey(m => m.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
