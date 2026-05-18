using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config.Rewards;

public sealed class InventoryItemDefinitionConfig : IEntityTypeConfiguration<CatalogItem>
{
    public void Configure(EntityTypeBuilder<CatalogItem> b)
    {
        b.ToTable("catalog_items");

        b.HasKey(x => x.Id);

        #region Properties
        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        
        b.Property(g => g.PublicId)
            .HasColumnName("public_id")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.Code)
            .HasColumnName("code")
            .IsRequired()
            .HasMaxLength(128);
        
        b.Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(256);
        
        b.Property(x => x.Description)
            .HasColumnName("description")
            .IsRequired(false)
            .HasMaxLength(4096);
        
        b.Property(x => x.Category)
            .HasColumnName("category")
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(CatalogItemCategory.Ticket);
        
        b.Property(x => x.MonetaryValue)
            .HasColumnName("monetary_value")
            .IsRequired(false)
            .HasPrecision(18, 3);

        b.Property(x => x.IconId)
            .HasColumnName("icon_id")
            .IsRequired(false);
        
        b.Property(x => x.IconValue)
            .HasColumnName("icon_value")
            .IsRequired(false)
            .HasMaxLength(2048);
        
        b.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired();
        
        b.Property(x => x.SortOrder)
            .HasColumnName("sort_order")
            .IsRequired();
        
        b.Property(x => x.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);
        #endregion

        #region Indexes
        b.HasIndex(x => x.PublicId)
            .IsUnique()
            .HasDatabaseName("ux_catalog_items_public_id");

        b.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("ux_catalog_items_code")
            .HasFilter("deleted_at IS NULL");

        b.HasIndex(x => x.Category)
            .HasDatabaseName("ix_catalog_items_category");

        b.HasIndex(x => x.IsActive)
            .HasDatabaseName("ix_catalog_items_active");
        #endregion

        #region Relationship
        b.HasOne(x => x.Icon)
            .WithMany()
            .HasForeignKey(x => x.IconId)
            .OnDelete(DeleteBehavior.SetNull);
        #endregion
        
        b.HasQueryFilter(x => x.DeletedAt == null);
    }
}