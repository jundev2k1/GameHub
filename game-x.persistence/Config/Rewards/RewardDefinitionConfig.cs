using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config.Rewards;

public sealed class RewardDefinitionConfig : IEntityTypeConfiguration<RewardDefinition>
{
    public void Configure(EntityTypeBuilder<RewardDefinition> b)
    {
        b.ToTable("reward_definitions");

        b.HasKey(x => x.Id);

        #region Properties
        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        b.Property(x => x.PublicId)
            .HasColumnName("public_id")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.Code)
            .HasColumnName("code")
            .HasMaxLength(128)
            .IsRequired();

        b.Property(x => x.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired()
            .HasDefaultValue(RewardItemType.None);

        b.Property(x => x.CatalogItemId)
            .HasColumnName("catalog_item_id")
            .IsRequired(false);

        b.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(256)
            .IsRequired();

        b.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(4096)
            .IsRequired(false);

        b.Property(x => x.Amount)
            .HasColumnName("amount")
            .HasPrecision(18, 4)
            .IsRequired(false);

        b.Property(x => x.Metadata)
            .HasColumnName("metadata")
            .HasColumnType("jsonb")
            .IsRequired(false);

        b.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

        b.Property(x => x.DeletedAt)
            .HasColumnName("deleted_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        #endregion

        #region Indexes

        b.HasIndex(x => x.PublicId)
            .IsUnique()
            .HasDatabaseName("ux_reward_definitions_public_id");

        b.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("ux_reward_definitions_code")
            .HasFilter("deleted_at IS NULL");

        b.HasIndex(x => x.Type)
            .HasDatabaseName("ix_reward_definitions_type")
            .HasFilter("deleted_at IS NULL");

        b.HasIndex(x => x.CatalogItemId)
            .HasDatabaseName("ix_reward_definitions_catalog_item")
            .HasFilter("deleted_at IS NULL");

        #endregion

        #region Relationships
        b.HasOne(x => x.CatalogItem)
            .WithMany(x => x.RewardDefinitions)
            .HasForeignKey(x => x.CatalogItemId)
            .OnDelete(DeleteBehavior.Restrict);
        #endregion
        
        b.HasQueryFilter(x => x.DeletedAt == null);
    }
}