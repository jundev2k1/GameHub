using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config.Rewards;

public sealed class UserRewardConfig : IEntityTypeConfiguration<UserReward>
{
    public void Configure(EntityTypeBuilder<UserReward> b)
    {
        b.ToTable("user_rewards");

        b.HasKey(x => x.Id);

        #region Properties

        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        b.Property(x => x.PublicId)
            .HasColumnName("public_id")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired()
            .HasMaxLength(36);

        b.Property(x => x.ExecutionId)
            .HasColumnName("execution_id")
            .IsRequired();

        b.Property(x => x.RewardDefinitionId)
            .HasColumnName("reward_definition_id")
            .IsRequired(false);

        b.Property(x => x.RewardPoolItemId)
            .HasColumnName("reward_pool_item_id")
            .IsRequired(false);

        b.Property(x => x.TransactionId)
            .HasColumnName("transaction_id")
            .IsRequired(false);

        b.Property(x => x.CatalogItemId)
            .HasColumnName("catalog_item_id")
            .IsRequired(false);

        b.Property(x => x.RewardType)
            .HasColumnName("reward_type")
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(64)
            .HasDefaultValue(RewardItemType.None);

        b.Property(x => x.Amount)
            .HasColumnName("amount")
            .IsRequired()
            .HasPrecision(18, 3);

        b.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(256)
            .IsRequired(false);

        b.Property(x => x.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(64)
            .HasDefaultValue(UserRewardStatus.Granted);

        b.Property(x => x.Metadata)
            .HasColumnName("metadata")
            .HasColumnType("jsonb")
            .IsRequired(false);

        b.Property(x => x.GrantedAt)
            .HasColumnName("granted_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        b.Property(x => x.ClaimedAt)
            .HasColumnName("claimed_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        b.Property(x => x.ExpiredAt)
            .HasColumnName("expired_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        b.Property(x => x.RevokedAt)
            .HasColumnName("revoked_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        #endregion

        #region Indexes

        b.HasIndex(x => x.PublicId)
            .IsUnique()
            .HasDatabaseName("ux_user_rewards_public_id");

        b.HasIndex(x => new { x.UserId, x.Status })
            .HasDatabaseName("ix_user_rewards_user_status");

        b.HasIndex(x => new { x.Status, x.ExpiredAt })
            .HasDatabaseName("ix_user_rewards_status_expired");

        b.HasIndex(x => x.ExecutionId)
            .HasDatabaseName("ix_user_rewards_execution");

        b.HasIndex(x => x.RewardDefinitionId)
            .HasDatabaseName("ix_user_rewards_definition");

        b.HasIndex(x => x.RewardPoolItemId)
            .HasDatabaseName("ix_user_rewards_pool_item");

        b.HasIndex(x => x.CatalogItemId)
            .HasDatabaseName("ix_user_rewards_catalog_item");

        b.HasIndex(x => x.TransactionId)
            .HasDatabaseName("ix_user_rewards_transaction");

        #endregion

        #region Relationships

        b.HasOne(x => x.User)
            .WithMany(x => x.UserRewards)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Execution)
            .WithMany(x => x.UserRewards)
            .HasForeignKey(x => x.ExecutionId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.RewardDefinition)
            .WithMany(x => x.UserRewards)
            .HasForeignKey(x => x.RewardDefinitionId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(x => x.RewardPoolItem)
            .WithMany(x => x.UserRewards)
            .HasForeignKey(x => x.RewardPoolItemId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(x => x.Transaction)
            .WithMany()
            .HasForeignKey(x => x.TransactionId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(x => x.CatalogItem)
            .WithMany()
            .HasForeignKey(x => x.CatalogItemId)
            .OnDelete(DeleteBehavior.SetNull);

        #endregion
    }
}