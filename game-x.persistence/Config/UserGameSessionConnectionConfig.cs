using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class UserGameSessionConnectionConfig : IEntityTypeConfiguration<UserGameSessionConnection>
{
    public void Configure(EntityTypeBuilder<UserGameSessionConnection> builder)
    {
        builder.ToTable("user_game_session_connections");

        builder.HasKey(ugsc => ugsc.Id);

        builder.Property(ugsc => ugsc.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(ugsc => ugsc.UserGameSessionId)
            .IsRequired();

        builder.Property(ugsc => ugsc.ConnectionId)
            .IsRequired();

        builder.Property(ugsc => ugsc.ConnectedAt)
            .IsRequired();

        builder.Property(ugsc => ugsc.DisconnectedAt)
            .IsRequired(false);

        builder.HasOne(ugsc => ugsc.Session)
            .WithMany(ugs => ugs.Connections)
            .HasForeignKey(ugsc => ugsc.UserGameSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ugsc => ugsc.ConnectionId);
        builder.HasIndex(ugs => new { ugs.ConnectedAt, ugs.DisconnectedAt });
    }
}
