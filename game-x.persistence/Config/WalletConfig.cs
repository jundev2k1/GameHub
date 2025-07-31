using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public class WalletConfig : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("wallets");
        
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(x => x.PublicId).IsUnique();
        builder.HasIndex(w => new { w.UserId, w.Network }).IsUnique();
        builder.HasIndex(w => w.WalletAddress).IsUnique();

        builder.Property(x => x.PublicId)
            .HasColumnName("public_id")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");
        
        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired(false);

        builder.Property(x => x.Network)
            .HasColumnName("network")
            .IsRequired();
        
        builder.Property(x => x.WalletAddress)
            .HasColumnName("wallet_address")
            .IsRequired();
        
        builder.HasOne(w => w.User)
            .WithMany(u => u.Wallets)
            .HasForeignKey(w => w.UserId);
    }
}