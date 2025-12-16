using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class TalentWalletTransactionConfig : IEntityTypeConfiguration<TalentWalletTransaction>
{
    public void Configure(EntityTypeBuilder<TalentWalletTransaction> builder)
    {
        builder.ToTable("talent_wallet_transactions");

        builder.HasKey(twt => twt.Id);

        builder.Property(twt => twt.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(twt => twt.PublicId)
            .HasColumnName("code")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(twt => twt.TalentId)
            .IsRequired();

        builder.Property(twt => twt.Amount)
            .IsRequired();

        builder.Property(twt => twt.Type)
            .IsRequired()
            .HasConversion<short>();

        builder.Property(twt => twt.BalanceAfter)
            .IsRequired();

        builder.Property(twt => twt.ReferenceId)
            .IsRequired(false);

        builder.Property(twt => twt.AdjustedBy)
            .IsRequired(false);

        builder.HasIndex(twt => twt.PublicId).IsUnique();

        builder.HasIndex(twt => twt.Type);
        builder.HasIndex(twt => twt.BalanceAfter);
        builder.HasIndex(twt => twt.CreatedAt);

        builder.HasOne(twt => twt.TalentWallet)
            .WithMany(tw => tw.Transactions)
            .HasForeignKey(twt => twt.TalentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
