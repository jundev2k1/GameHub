using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class UserBalanceConfig: IEntityTypeConfiguration<UserBalance>
{
    public void Configure(EntityTypeBuilder<UserBalance> builder)
    {
        builder.ToTable("user_balances");
        
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(x => x.PublicId).IsUnique();
        // Create a unique index to avoid multiple balances of the same currency for the same person
        builder.HasIndex(x => new { x.UserId, x.CryptoTokenId }).IsUnique();

        builder.Property(x => x.PublicId)
            .HasColumnName("public_id")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");
        
        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();
        
        builder.Property(x => x.CryptoTokenId)
            .HasColumnName("crypto_token_id")
            .IsRequired();
        
        builder.Property(x => x.Amount)
            .HasColumnName("amount")
            .IsRequired();
        
        builder.Property(x => x.FrozenAmount)
            .HasColumnName("frozen_amount")
            .IsRequired();
        
        builder.Property(x => x.Version)
            .HasColumnName("version")
            .IsRequired()
            .IsRowVersion();
        
        builder.HasOne(x => x.CryptoToken)
            .WithMany()
            .HasForeignKey(x => x.CryptoTokenId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserBalances)
            .HasForeignKey(x => x.UserId);
    }
}