using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public class UserUsdtLedgerConfig : IEntityTypeConfiguration<UserUsdtLedger>
{
    public void Configure(EntityTypeBuilder<UserUsdtLedger> builder)
    {
        builder.ToTable("user_usdt_ledgers");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.PublicId).IsUnique();

        builder.Property(x => x.PublicId)
            .HasColumnName("public_id")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired(false);

        builder.Property(x => x.Timestamp)
            .HasColumnName("timestamp")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.FlowType)
            .HasColumnName("flow_type")
            .IsRequired();

        builder.Property(x => x.SourceId)
            .HasColumnName("source_id")
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(x => x.ChangeAmount)
            .HasColumnName("change_amount")
            .IsRequired();

        builder.Property(x => x.BalanceAfter)
            .HasColumnName("balance_after")
            .IsRequired();

        builder.Property(x => x.StatusAtEvent)
            .HasColumnName("status_at_event")
            .IsRequired(false)
            .HasDefaultValue(string.Empty);

        builder.Property(x => x.ChainTransactionId)
            .HasColumnName("chain_transaction_id")
            .IsRequired(false);

        // NEW: Configuration cho Type field
        builder.Property(x => x.Type)
            .HasColumnName("type")
            .IsRequired()
            .HasDefaultValue(LedgerType.Uxm); // Default = 1

        // NEW: Configuration cho GameTransactionId field  
        builder.Property(x => x.GameTransactionId)
            .HasColumnName("game_transaction_id")
            .IsRequired(false);

        builder.Property(x => x.Meta)
            .HasColumnName("meta")
            .HasColumnType("jsonb")
            .IsRequired()
            .HasDefaultValue("{}");

        builder.HasOne(x => x.User)
            .WithMany(u => u.UserUsdtLedgers)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasOne(x => x.ChainTransaction)
            .WithOne(x => x.Ledger)
            .HasForeignKey<UserUsdtLedger>(x => x.ChainTransactionId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // NEW: Relationship với GameTransaction
        builder.HasOne(x => x.GameTransaction)
            .WithMany()
            .HasForeignKey(x => x.GameTransactionId)
            .OnDelete(DeleteBehavior.SetNull);

        // NEW: Indexes để optimize queries
        builder.HasIndex(x => new { x.UserId, x.Type, x.Timestamp });
        builder.HasIndex(x => x.GameTransactionId);
    }
}