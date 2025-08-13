using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class FiatCurrencyConfig : IEntityTypeConfiguration<FiatCurrency>
{
    public void Configure(EntityTypeBuilder<FiatCurrency> builder)
    {
        builder.ToTable("fiat_currencies");

        builder.HasKey(fc => fc.Id);

        builder.Property(fc => fc.PublicId)
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(fc => fc.Code)
            .IsRequired()
            .HasConversion(fc => fc.Value, fc => CurrencyUnit.Of(fc));

        builder.Property(fc => fc.Name)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue(string.Empty);

        builder.Property(fc => fc.Symbol)
            .IsRequired()
            .HasMaxLength(30)
            .HasDefaultValue(string.Empty);

        builder.Property(fc => fc.Description)
            .IsRequired()
            .HasMaxLength(255)
            .HasDefaultValue(string.Empty);

        builder.Property(fc => fc.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(x => x.PublicId).IsUnique();
    }
}
