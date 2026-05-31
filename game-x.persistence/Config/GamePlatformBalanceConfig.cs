using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class GamePlatformBalanceConfig : IEntityTypeConfiguration<GamePlatformBalance>
{
    public void Configure(EntityTypeBuilder<GamePlatformBalance> builder)
    {
        builder.ToTable("game_platform_balances");

        builder.HasKey(gpb => gpb.Id);

        builder.Property(gpb => gpb.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(gpb => gpb.UserId)
            .IsRequired();

        builder.Property(gpb => gpb.PlatformId)
            .IsRequired();

        builder.Property(gpb => gpb.AvailableBalance)
            .IsRequired();

        builder.Property(gpb => gpb.LockedBalance)
            .IsRequired();

        builder.Property(gpb => gpb.LastSyncedAt)
            .IsRequired();

        builder.HasIndex(gpb => new { gpb.UserId, gpb.PlatformId }).IsUnique();

        builder.HasOne(gpb => gpb.Platform)
            .WithMany(gp => gp.Balances)
            .HasForeignKey(gpb => gpb.PlatformId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
