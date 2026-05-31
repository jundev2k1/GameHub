using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class AsymmetricKeyConfig : IEntityTypeConfiguration<AsymmetricKey>
{
    public void Configure(EntityTypeBuilder<AsymmetricKey> builder)
    {
        builder.ToTable("asymmetric_keys");

        builder.Property(ak => ak.Name)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue(string.Empty);

        builder.Property(ak => ak.KeyValue)
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(ak => ak.KeyType)
            .IsRequired()
            .HasConversion<short>();

        builder.Property(ak => ak.Algorithm)
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(ak => ak.Description)
            .IsRequired()
            .HasMaxLength(255)
            .HasDefaultValue(string.Empty);

        builder.HasIndex(x => new { x.Name, x.KeyType, x.Algorithm }).IsUnique();
    }
}
