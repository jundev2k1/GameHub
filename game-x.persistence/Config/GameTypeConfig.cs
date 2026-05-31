using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class GameTypeConfig : IEntityTypeConfiguration<GameType>
{
    public void Configure(EntityTypeBuilder<GameType> builder)
    {
        builder.ToTable("game_types");

        builder.HasKey(gt => gt.Id);

        builder.Property(gt => gt.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(gt => gt.PublicId)
            .HasColumnName("code")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(gt => gt.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasDefaultValue(string.Empty);

        builder.Property(gt => gt.Description)
            .IsRequired()
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty);

        builder.Property(gt => gt.Note)
            .IsRequired()
            .HasMaxLength(4000)
            .HasDefaultValue(string.Empty);

        builder.Property(gt => gt.Priority)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(gt => gt.IsActive)
            .IsRequired()
            .HasConversion<short>()
            .HasDefaultValue(true);

        builder.HasIndex(gt => gt.PublicId).IsUnique();
    }
}
