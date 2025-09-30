using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class LiveStreamGiftPriceConfig : IEntityTypeConfiguration<LiveStreamGiftPrice>
{
    public void Configure(EntityTypeBuilder<LiveStreamGiftPrice> builder)
    {
        builder.ToTable("livestream_gift_prices");

        builder.HasKey(lsgp => new { lsgp.LiveStreamGiftId, lsgp.CryptoTokenId });

        builder.Property(lsgp => lsgp.LiveStreamGiftId)
            .IsRequired();

        builder.Property(lsgp => lsgp.CryptoTokenId)
            .IsRequired();

        builder.Property(lsgp => lsgp.TokenCost)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(lsgp => lsgp.IsActive)
            .IsRequired();

        builder.Ignore(lsgp => lsgp.Id);

        builder.HasOne(lsgp => lsgp.LiveStreamGift)
            .WithMany(lsg => lsg.GiftPrices)
            .HasForeignKey(lsgp => lsgp.LiveStreamGiftId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(lsgp => lsgp.CryptoToken)
            .WithMany()
            .HasForeignKey(lsgp => lsgp.CryptoTokenId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
