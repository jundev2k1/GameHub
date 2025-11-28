using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class SystemWalletTransactionConfig : IEntityTypeConfiguration<SystemWalletTransaction>
{
    public void Configure(EntityTypeBuilder<SystemWalletTransaction> builder)
    {
        builder.ToTable("talent_wallet_transactions");

        builder.HasKey(twt => twt.Id);

        builder.Property(twt => twt.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(twt => twt.WalletId)
            .IsRequired();

        builder.Property(twt => twt.Amount)
            .IsRequired();

        builder.Property(twt => twt.BalanceAfter)
            .IsRequired(false);

        builder.Property(twt => twt.ReferenceId)
            .IsRequired(false);

        builder.HasIndex(twt => twt.BalanceAfter);
        builder.HasIndex(twt => twt.CreatedAt);

        builder.HasOne(swt => swt.Wallet)
            .WithMany(sw => sw.Transactions)
            .HasForeignKey(twt => twt.WalletId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
