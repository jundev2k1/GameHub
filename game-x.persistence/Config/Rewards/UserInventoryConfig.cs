using game_x.domain.Entities.Rewards;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config.Rewards;

public sealed class InventoryConfig : IEntityTypeConfiguration<UserInventory>
{
    public void Configure(EntityTypeBuilder<UserInventory> b)
    {
        b.ToTable("user_inventories");

        b.HasKey(x => new { x.UserId, x.CatalogItemId });

        #region Properties
        b.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired()
            .HasMaxLength(36);

        b.Property(x => x.CatalogItemId)
            .HasColumnName("catalog_item_id")
            .IsRequired();

        b.Property(x => x.Quantity)
            .HasColumnName("quantity")
            .IsRequired()
            .HasDefaultValue(0);
        #endregion

        #region Indexes
        b.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_inventories_user");

        b.HasIndex(x => x.CatalogItemId)
            .HasDatabaseName("ix_inventories_item");

        b.HasIndex(x => new { x.UserId, x.CatalogItemId })
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
            .HasForeignKey(x => x.CatalogItemId)
            .OnDelete(DeleteBehavior.Restrict);
        #endregion
    }
}