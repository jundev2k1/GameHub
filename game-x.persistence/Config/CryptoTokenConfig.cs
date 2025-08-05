using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class CryptoTokenConfig : IEntityTypeConfiguration<CryptoToken>
{
    public void Configure(EntityTypeBuilder<CryptoToken> builder)
    {
        builder.ToTable("crypto_tokens");
        
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(t => new { t.Symbol, t.Network }).IsUnique();
        
        builder.Property(x => x.Symbol)
            .HasColumnName("symbol")
            .IsRequired()
            .HasDefaultValue(string.Empty);
        
        builder.Property(x => x.Network)
            .HasColumnName("network")
            .IsRequired()
            .HasDefaultValue(NetworkType.Tron);
        
        builder.Property(x => x.ContractAddress)
            .HasColumnName("contract_address")
            .IsRequired()
            .HasDefaultValue(string.Empty);
    }
}