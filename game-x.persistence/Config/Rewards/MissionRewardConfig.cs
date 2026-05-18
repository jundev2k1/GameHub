using game_x.domain.Entities.Rewards;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config.Rewards;

public sealed class MissionRewardConfig : IEntityTypeConfiguration<MissionReward>
{
    public void Configure(EntityTypeBuilder<MissionReward> b)
    {
        b.ToTable("mission_rewards");

        b.HasKey(x => x.Id);

        #region Properties
        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        b.Property(x => x.PublicId)
            .HasColumnName("public_id")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.MissionId)
            .HasColumnName("mission_id")
            .IsRequired();

        b.Property(x => x.RewardDefinitionId)
            .HasColumnName("reward_definition_id")
            .IsRequired();

        b.Property(x => x.Sequence)
            .HasColumnName("sequence")
            .IsRequired();
        
        b.Property(x => x.SortOrder)
            .HasColumnName("sort_order")
            .IsRequired();
        
        b.Property(x => x.RequiredProgress)
            .HasColumnName("required_progress")
            .IsRequired();

        b.Property(x => x.IsClaimable)
            .HasColumnName("is_claimable")
            .IsRequired()
            .HasDefaultValue(true);
        
        b.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

        b.Property(x => x.StartAt)
            .HasColumnName("start_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
        
        b.Property(x => x.EndAt)
            .HasColumnName("end_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
        
        b.Property(x => x.DeletedAt)
            .HasColumnName("deleted_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
        #endregion

        #region Indexes
        b.HasIndex(x => x.PublicId)
            .IsUnique()
            .HasDatabaseName("ux_mission_rewards_public_id");

        b.HasIndex(x => new { x.MissionId, x.Sequence })
            .HasDatabaseName("ix_mission_rewards_mission_sequence")
            .HasFilter("deleted_at IS NULL");

        b.HasIndex(x => new { x.MissionId, x.RequiredProgress })
            .HasDatabaseName("ix_mission_rewards_mission_progress")
            .HasFilter("deleted_at IS NULL");

        b.HasIndex(x => new { x.MissionId, x.SortOrder })
            .HasDatabaseName("ix_mission_rewards_sort_order")
            .HasFilter("deleted_at IS NULL");
        #endregion

        #region Relationships
        b.HasOne(x => x.Mission)
            .WithMany(x => x.MissionRewards)
            .HasForeignKey(x => x.MissionId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.RewardDefinition)
            .WithMany(x => x.MissionRewards)
            .HasForeignKey(x => x.RewardDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);
        #endregion
        
        b.HasQueryFilter(x => x.DeletedAt == null);
    }
}