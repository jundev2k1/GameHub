using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public class BalanceTransferLogConfig: IEntityTypeConfiguration<BalanceTransferLog>
{
    public void Configure(EntityTypeBuilder<BalanceTransferLog> builder)
    {
        builder.ToTable("balance_transfer_logs");
        
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(x => x.PublicId).IsUnique();

        builder.Property(x => x.PublicId)
            .HasColumnName("public_id")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");
        
        builder.Property(x => x.FromUserId)
            .HasColumnName("from_user_id")
            .IsRequired();
        
        builder.Property(x => x.ToUserId)
            .HasColumnName("to_user_id")
            .IsRequired();
        
        builder.Property(x => x.CryptoTokenId)
            .HasColumnName("crypto_token_id")
            .IsRequired();
        
        builder.Property(x => x.Amount)
            .HasColumnName("amount")
            .IsRequired();
        
        builder.Property(x => x.Fee)
            .HasColumnName("fee")
            .IsRequired();
        
        builder.Property(x => x.Note)
            .HasColumnName("note")
            .IsRequired(false);
        
        builder.Property(w => w.Version)
            .HasColumnName("version")
            .IsRequired()
            .IsRowVersion();
        
        builder.HasOne(b => b.CryptoToken)
            .WithMany()
            .HasForeignKey(b => b.CryptoTokenId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.FromUser)
            .WithMany(u => u.BalanceTransferLogs)
            .HasForeignKey(t => t.FromUserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(t => t.ToUser)
            .WithMany()
            .HasForeignKey(t => t.ToUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}