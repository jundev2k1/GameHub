using game_x.domain.Entities.Rewards;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config.Rewards;

public sealed class RewardPoolItemConfig : IEntityTypeConfiguration<RewardPoolItem>
{
    public void Configure(EntityTypeBuilder<RewardPoolItem> b)
    {
        b.ToTable("reward_pool_items");

        b.HasKey(x => x.Id);

        #region Properties
        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        
        b.Property(g => g.PublicId)
            .HasColumnName("public_id")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");
        
        b.Property(x => x.RewardPoolId)
            .HasColumnName("reward_pool_id");

        b.Property(x => x.RewardDefinitionId)
            .HasColumnName("reward_definition_id")
            .IsRequired();
        
        b.Property(x => x.Weight)
            .HasColumnName("weight");
        
        b.Property(x => x.SortOrder)
            .HasColumnName("sort_order");
        
        b.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);
        
        b.Property(x => x.StartAt)
            .HasColumnName("start_at")
            .IsRequired(false)
            .HasColumnType("timestamp with time zone");

        b.Property(x => x.EndAt)
            .HasColumnName("end_at")
            .IsRequired(false)
            .HasColumnType("timestamp with time zone");

        b.Property(x => x.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false)
            .HasColumnType("timestamp with time zone");
        #endregion
        
        #region Indexes
        b.HasIndex(x => new { x.RewardPoolId, x.RewardDefinitionId })
            .IsUnique()
            .HasDatabaseName("ux_reward_pool_items_pool_reward_definition")
            .HasFilter("deleted_at IS NULL");

        b.HasIndex(x => x.RewardPoolId)
            .HasDatabaseName("ix_reward_pool_items_pool")
            .HasFilter("deleted_at IS NULL");

        b.HasIndex(x => new { x.StartAt, x.EndAt })
            .HasDatabaseName("ix_reward_pool_items_active_range")
            .HasFilter("deleted_at IS NULL AND is_active = true");

        b.HasIndex(x => new { x.RewardPoolId, x.Weight })
            .HasDatabaseName("ix_reward_pool_items_pool_weight")
            .HasFilter("deleted_at IS NULL AND is_active = true");
        
        b.HasIndex(x => new { x.RewardPoolId, x.SortOrder })
            .HasDatabaseName("ix_reward_pool_items_sort_order")
            .HasFilter("deleted_at IS NULL AND is_active = true");
        #endregion

        #region Relationship
        b.HasOne(x => x.RewardPool)
            .WithMany(x => x.RewardPoolItems)
            .HasForeignKey(x => x.RewardPoolId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.RewardDefinition)
            .WithMany(x => x.RewardPoolItems)
            .HasForeignKey(x => x.RewardDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);
        #endregion
        
        b.HasQueryFilter(x => x.DeletedAt == null);
    }
}