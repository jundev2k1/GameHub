using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public class GameCategoryConfig : IEntityTypeConfiguration<GameCategory>
{
    public void Configure(EntityTypeBuilder<GameCategory> builder)
    {
        builder.ToTable("game_categories");

        builder.HasKey(gc => gc.Id);

        builder.Property(gc => gc.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(gc => gc.PublicId)
            .HasColumnName("code")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(gc => gc.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasDefaultValue(string.Empty);

        builder.Property(gc => gc.Description)
            .IsRequired()
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty);

        builder.Property(gc => gc.Note)
            .IsRequired()
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty);

        builder.Property(gc => gc.Priority)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(gc => gc.IsActive)
            .IsRequired()
            .HasConversion<short>()
            .HasDefaultValue(true);

        builder.HasIndex(gc => gc.PublicId).IsUnique();
    }
}
