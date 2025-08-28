using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class TransactionConfig : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions");
        
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(x => x.PublicId).IsUnique();

        builder.Property(x => x.PublicId)
            .HasColumnName("public_id")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");
        
        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();
        
        builder.Property(x => x.Amount)
            .HasColumnName("amount")
            .IsRequired();
        
        builder.Property(x => x.Fee)
            .HasColumnName("fee")
            .IsRequired();
        
        builder.Property(x => x.CryptoTokenId)
            .HasColumnName("crypto_token_id")
            .IsRequired();
        
        builder.Property(x => x.Type)
            .HasColumnName("type")
            .IsRequired()
            .HasDefaultValue(TransactionType.Init);
        
        builder.Property(x => x.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasDefaultValue(TransactionStatus.Pending);
        
        builder.Property(x => x.BalanceAfter)
            .HasColumnName("balance_after")
            .IsRequired(false);
        
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
            .WithMany(u => u.Transactions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasOne(x => x.TransactionInternal)
            .WithOne(x => x.Transaction)
            .HasForeignKey<Transaction>(x => x.TransactionInternalId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasOne(x => x.TransactionExternal)
            .WithOne(x => x.Transaction)
            .HasForeignKey<Transaction>(x => x.TransactionExternalId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}