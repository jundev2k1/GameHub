using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class UserGameSessionConfig : IEntityTypeConfiguration<UserGameSession>
{
    public void Configure(EntityTypeBuilder<UserGameSession> builder)
    {
        builder.ToTable("user_game_sessions");

        builder.HasKey(ugs => ugs.Id);

        builder.Property(ugs => ugs.Id)
            .IsRequired();

        builder.Property(ugs => ugs.UserId)
            .IsRequired();

        builder.Property(ugs => ugs.PlatformId)
            .IsRequired();

        builder.Property(ugs => ugs.GameId)
            .IsRequired(false);

        builder.Property(ugs => ugs.IsEnd)
            .IsRequired();

        builder.Property(ugs => ugs.BalanceSnapshot)
            .IsRequired();

        builder.HasOne(ugs => ugs.User)
            .WithMany()
            .HasForeignKey(ugs => ugs.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ugs => ugs.Platform)
            .WithMany()
            .HasForeignKey(ugs => ugs.PlatformId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ugs => ugs.Game)
            .WithMany()
            .HasForeignKey(ugs => ugs.GameId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(ugs => new { ugs.PlatformId, ugs.GameId });
        builder.HasIndex(ugs => ugs.BalanceSnapshot);
        builder.HasIndex(ugs => ugs.IsEnd);
    }
}
