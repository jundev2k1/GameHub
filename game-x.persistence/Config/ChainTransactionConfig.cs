using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class ChainTransactionConfig : IEntityTypeConfiguration<ChainTransaction>
{
    public void Configure(EntityTypeBuilder<ChainTransaction> builder)
    {
        builder.ToTable("chain_transactions");
        
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(x => x.Hash).IsUnique();
        builder.HasIndex(x => x.OrderNumber).IsUnique();
        builder.HasIndex(x => x.PublicId).IsUnique();

        builder.Property(x => x.PublicId)
            .HasColumnName("public_id")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");
        
        builder.Property(o => o.OrderUid)
            .HasColumnName("order_uid")
            .IsRequired()
            .HasDefaultValue(string.Empty);
        
        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired(false);

        builder.Property(x => x.OrderNumber)
            .HasColumnName("order_number")
            .IsRequired()
            .HasDefaultValue(string.Empty);
        
        builder.Property(x => x.Hash)
            .HasColumnName("hash")
            .IsRequired(false);
        
        builder.Property(al => al.FromAddress)
            .HasColumnName("from_address")
            .IsRequired(false);
        
        builder.Property(x => x.ToAddress)
            .HasColumnName("to_address")
            .IsRequired(false);
        
        builder.Property(x => x.Amount)
            .HasColumnName("amount")
            .IsRequired();
        
        builder.Property(x => x.Fee)
            .HasColumnName("fee")
            .IsRequired();
        
        builder.Property(x => x.CryptoTokenId)
            .HasColumnName("crypto_token_id")
            .IsRequired();
        
        builder.Property(x => x.ConfirmedAt)
            .HasColumnName("confirmed_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        
        builder.Property(x => x.Type)
            .HasColumnName("type")
            .IsRequired();
        
        builder.Property(x => x.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasDefaultValue(ChainTransactionStatus.Pending);
        
        builder.Property(x => x.Meta)
            .HasColumnName("meta")
            .HasColumnType("jsonb")
            .IsRequired()
            .HasDefaultValue("{}");
        
        builder.Property(x => x.Note)
            .HasColumnName("note")
            .IsRequired(false);
        
        builder.HasOne(x => x.CryptoToken)
            .WithMany()
            .HasForeignKey(x => x.CryptoTokenId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany(u => u.ChainTransactions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}