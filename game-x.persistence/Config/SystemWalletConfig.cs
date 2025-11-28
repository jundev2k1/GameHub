using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class SystemWalletConfig : IEntityTypeConfiguration<SystemWallet>
{
    public void Configure(EntityTypeBuilder<SystemWallet> builder)
    {
        builder.ToTable("system_wallets");

        builder.HasKey(sw => sw.Id);

        builder.Property(sw => sw.Id)
            .IsRequired();

        builder.Property(sw => sw.Type)
            .IsRequired()
            .HasConversion<short>();

        builder.HasIndex(sw => sw.Balance);
    }
}
