using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class TalentWalletConfig : IEntityTypeConfiguration<TalentWallet>
{
    public void Configure(EntityTypeBuilder<TalentWallet> builder)
    {
        builder.ToTable("talent_wallets");

        builder.HasKey(tw => tw.Id);

        builder.Property(tw => tw.Id)
            .IsRequired();

        builder.Property(tw => tw.Balance)
            .IsRequired();

        builder.HasIndex(tw => tw.Balance);

        builder.HasOne(tw => tw.Talent)
            .WithOne(u => u.TalentWallet)
            .HasForeignKey<TalentWallet>(tw => tw.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
