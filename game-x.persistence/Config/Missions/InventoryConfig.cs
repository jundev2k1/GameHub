using game_x.domain.Entities.Missions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config.Missions;

public sealed class InventoryConfig : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> b)
    {
        b.ToTable("inventories");

        b.HasKey(x => new { x.UserId, x.InventoryItemDefinitionId });

        #region Properties
        b.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired()
            .HasMaxLength(36);

        b.Property(x => x.InventoryItemDefinitionId)
            .HasColumnName("inventory_item_definition_id")
            .IsRequired();

        b.Property(x => x.Quantity)
            .HasColumnName("quantity")
            .IsRequired()
            .HasDefaultValue(0);
        #endregion

        #region Indexes
        b.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_inventories_user");

        b.HasIndex(x => x.InventoryItemDefinitionId)
            .HasDatabaseName("ix_inventories_item");

        b.HasIndex(x => new
            {
                x.UserId,
                x.InventoryItemDefinitionId
            })
            .IsUnique()
            .HasDatabaseName("ux_inventories_user_item");
        #endregion

        #region Relationships
        b.HasOne(x => x.User)
            .WithMany(x => x.Inventories)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Item)
            .WithMany()
            .HasForeignKey(x => x.InventoryItemDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);
        #endregion
    }
}