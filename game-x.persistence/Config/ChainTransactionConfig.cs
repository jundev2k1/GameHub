using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class ChainTransactionConfig : IEntityTypeConfiguration<ChainTransaction>
{
    public void Configure(EntityTypeBuilder<ChainTransaction> builder)
    {
        builder.ToTable("chain_transactions");
        
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(x => x.PublicId).IsUnique();
        builder.HasIndex(x => x.TransactionHash).IsUnique();

        builder.Property(al => al.PublicId)
            .HasColumnName("public_id")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");
        
        builder.Property(al => al.UserId)
            .HasColumnName("user_id")
            .IsRequired(false);

        builder.Property(al => al.OrderNumber)
            .HasColumnName("order_number")
            .IsRequired()
            .HasDefaultValue(string.Empty);
        
        builder.Property(al => al.TransactionHash)
            .HasColumnName("transaction_hash")
            .HasMaxLength(100)
            .IsRequired(false);
        
        builder.Property(al => al.FromAddress)
            .HasColumnName("from_address")
            .IsRequired(false);
        
        builder.Property(al => al.ToAddress)
            .HasColumnName("to_address")
            .IsRequired(false);
        
        builder.Property(al => al.Amount)
            .HasColumnName("amount")
            .IsRequired();
        
        builder.Property(al => al.Fee)
            .HasColumnName("fee")
            .IsRequired();
        
        builder.Property(al => al.CryptoTokenId)
            .HasColumnName("crypto_token_id")
            .IsRequired();
        
        builder.Property(o => o.ConfirmedAt)
            .HasColumnName("confirmed_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        
        builder.Property(al => al.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasDefaultValue(ChainTransactionStatus.Pending);
        
        builder.Property(al => al.Meta)
            .HasColumnName("meta")
            .HasColumnType("jsonb")
            .IsRequired()
            .HasDefaultValue("{}");
        
        builder.Property(al => al.Note)
            .HasColumnName("note")
            .IsRequired(false);
        
        builder.HasOne(x => x.CryptoToken)
            .WithMany()
            .HasForeignKey(x => x.CryptoTokenId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.User)
            .WithMany(u => u.ChainTransactions)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}