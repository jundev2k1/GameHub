using game_x.domain.Entities.Missions;
using game_x.domain.Enum.Missions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config.Missions;

public sealed class RewardItemConfig : IEntityTypeConfiguration<RewardItem>
{
    public void Configure(EntityTypeBuilder<RewardItem> b)
    {
        b.ToTable("reward_items");

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

        b.Property(x => x.Code)
            .HasColumnName("code")
            .IsRequired()
            .HasMaxLength(128);

        b.Property(x => x.InventoryItemCode)
            .HasColumnName("inventory_item_code")
            .IsRequired(false)
            .HasMaxLength(256);
        
        b.Property(x => x.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .IsRequired()
            .HasDefaultValue(RewardItemType.InventoryItem);

        b.Property(x => x.Title)
            .HasColumnName("title")
            .IsRequired()
            .HasMaxLength(256)
            .HasDefaultValue(string.Empty);

        b.Property(x => x.Description)
            .HasColumnName("description")
            .IsRequired(false)
            .HasMaxLength(4096);

        b.Property(x => x.Amount)
            .HasColumnName("amount")
            .HasPrecision(18, 3);
        
        b.Property(x => x.SortOrder)
            .HasColumnName("sort_order");
        
        b.Property(x => x.IsActive)
            .HasColumnName("is_active");
        
        b.Property(x => x.Weight)
            .HasColumnName("weight");
        
        b.Property(x => x.Metadata)
            .HasColumnName("metadata")
            .HasColumnType("jsonb");
        
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
        b.HasIndex(x => new {x.RewardPoolId, x.Code})
            .IsUnique()
            .HasDatabaseName("ux_reward_items_pool_code")
            .HasFilter("deleted_at IS NULL");

        b.HasIndex(x => x.InventoryItemCode)
            .HasDatabaseName("ix_reward_items_inventory_type")
            .HasFilter("deleted_at IS NULL");

        b.HasIndex(x => new { x.StartAt, x.EndAt })
            .HasDatabaseName("ix_reward_items_active_range")
            .HasFilter("deleted_at IS NULL AND is_active = true");

        b.HasIndex(x => new { x.RewardPoolId, x.Weight })
            .HasDatabaseName("ix_reward_items_pool_weight")
            .HasFilter("deleted_at IS NULL AND is_active = true");
        
        b.HasIndex(x => new { x.RewardPoolId, x.SortOrder })
            .HasDatabaseName("ix_reward_items_sort_order")
            .HasFilter("deleted_at IS NULL AND is_active = true");
        #endregion

        #region Relationship
        b.HasOne(x => x.RewardPool)
            .WithMany(x => x.RewardItems)
            .HasForeignKey(x => x.RewardPoolId)
            .OnDelete(DeleteBehavior.Cascade);
        #endregion
    }
}