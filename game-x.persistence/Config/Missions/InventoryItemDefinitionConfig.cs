using game_x.domain.Entities.Missions;
using game_x.domain.Enum.Missions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config.Missions;

public sealed class InventoryItemDefinitionConfig : IEntityTypeConfiguration<InventoryItemDefinition>
{
    public void Configure(EntityTypeBuilder<InventoryItemDefinition> b)
    {
        b.ToTable("inventory_item_definitions");

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
            .HasDefaultValue(InventoryItemCategory.Ticket);
        
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
        #endregion

        #region Indexes
        b.HasIndex(x => x.PublicId)
            .IsUnique()
            .HasDatabaseName("ux_inventory_item_definitions_public_id");

        b.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("ux_inventory_item_definitions_code");

        b.HasIndex(x => x.Category)
            .HasDatabaseName("ix_inventory_item_definitions_category");

        b.HasIndex(x => x.IsActive)
            .HasDatabaseName("ix_inventory_item_definitions_active");
        #endregion

        #region Relationship
        b.HasOne(x => x.Icon)
            .WithMany()
            .HasForeignKey(x => x.IconId)
            .OnDelete(DeleteBehavior.SetNull);
        #endregion
    }
}