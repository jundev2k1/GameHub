using game_x.domain.Entities.Missions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config.Missions;

public sealed class IdempotencyKeyConfig : IEntityTypeConfiguration<IdempotencyKey>
{
    public void Configure(EntityTypeBuilder<IdempotencyKey> b)
    {
        b.ToTable("idempotency_keys");

        b.HasKey(x => x.Id);

        #region Properties
        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        b.Property(x => x.Key)
            .HasColumnName("key")
            .IsRequired()
            .HasMaxLength(256);

        b.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired()
            .HasMaxLength(36);

        b.Property(x => x.ActionType)
            .HasColumnName("action_type")
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        b.Property(x => x.ResponseMetadata)
            .HasColumnName("response_metadata")
            .HasColumnType("jsonb")
            .IsRequired(false);

        b.Property(x => x.ExpiredAt)
            .HasColumnName("expired_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
        #endregion

        #region Indexes
        b.HasIndex(x => x.Key)
            .IsUnique()
            .HasDatabaseName("ux_idempotency_keys_key");

        b.HasIndex(x => new { x.UserId, x.ActionType })
            .HasDatabaseName("ix_idempotency_keys_user_action");

        b.HasIndex(x => x.ExpiredAt)
            .HasDatabaseName("ix_idempotency_keys_expired");
        #endregion

        #region Relationships
        b.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        #endregion
    }
}