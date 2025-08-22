using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public class GamePlatformConfig : IEntityTypeConfiguration<GamePlatform>
{
    public void Configure(EntityTypeBuilder<GamePlatform> builder)
    {
        builder.ToTable("game_platforms");

        builder.HasKey(gp => gp.Id);

        builder.Property(gp => gp.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(gp => gp.PublicId)
            .HasColumnName("code")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(gp => gp.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasDefaultValue(string.Empty);

        builder.Property(gp => gp.Description)
            .IsRequired()
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty);

        builder.Property(gp => gp.Note)
            .IsRequired()
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty);

        builder.Property(gp => gp.Priority)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(gp => gp.IsActive)
            .IsRequired()
            .HasConversion<short>()
            .HasDefaultValue(true);

        builder.HasIndex(gp => gp.PublicId).IsUnique();

        builder.HasMany(gp => gp.Games)
            .WithOne(g => g.Platform)
            .HasForeignKey(g => g.PlatformId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
