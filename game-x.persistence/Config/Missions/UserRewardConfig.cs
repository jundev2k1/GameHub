using game_x.domain.Entities.Missions;
using game_x.domain.Enum.Missions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config.Missions;

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
        
        b.Property(g => g.PublicId)
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
        
        b.Property(x => x.RewardPoolId)
            .HasColumnName("reward_pool_id")
            .IsRequired();
        
        b.Property(x => x.TransactionId)
            .HasColumnName("transaction_id")
            .IsRequired(false);
        
        b.Property(x => x.InventoryItemDefinitionId)
            .HasColumnName("inventory_item_definition_id")
            .IsRequired(false);
        
        b.Property(x => x.RewardType)
            .HasColumnName("reward_type")
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(RewardItemType.None);

        b.Property(x => x.Amount)
            .HasColumnName("amount")
            .IsRequired()
            .HasPrecision(18, 3);

        b.Property(x => x.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(UserRewardStatus.Granted);

        b.Property(x => x.Metadata)
            .HasColumnName("metadata")
            .IsRequired(false)
            .HasColumnType("jsonb");

        b.Property(x => x.GrantedAt)
            .HasColumnName("granted_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        
        b.Property(x => x.ExpiredAt)
            .HasColumnName("expired_at")
            .HasColumnType("timestamp with time zone");
        
        b.Property(x => x.RevokedAt)
            .HasColumnName("revoked_at")
            .HasColumnType("timestamp with time zone");
        #endregion

        #region Indexes

        b.HasIndex(x => x.PublicId)
            .IsUnique()
            .HasDatabaseName("ux_user_rewards_public_id");

        b.HasIndex(x => new { x.UserId, x.GrantedAt })
            .HasDatabaseName("ix_user_rewards_user_granted");

        b.HasIndex(x => x.RewardPoolId)
            .HasDatabaseName("ix_user_rewards_pool");

        b.HasIndex(x => x.RewardItemId)
            .HasDatabaseName("ix_user_rewards_item");

        b.HasIndex(x => x.Status)
            .HasDatabaseName("ix_user_rewards_status");

        b.HasIndex(x => x.ExecutionId)
            .HasDatabaseName("ix_user_rewards_execution");
        #endregion

        #region Relationship
        b.HasOne(x => x.User)
            .WithMany(x => x.UserRewards)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Execution)
            .WithMany(x => x.UserRewards)
            .HasForeignKey(x => x.ExecutionId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.RewardPool)
            .WithMany()
            .HasForeignKey(x => x.RewardPoolId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.RewardItem)
            .WithMany()
            .HasForeignKey(x => x.RewardItemId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Transaction)
            .WithMany()
            .HasForeignKey(x => x.TransactionId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(x => x.InventoryItemDefinition)
            .WithMany()
            .HasForeignKey(x => x.InventoryItemDefinitionId)
            .OnDelete(DeleteBehavior.SetNull);
        #endregion
    }
}