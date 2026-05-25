using System.Text.Json;
using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;
using game_x.domain.ValueObjects.Missions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace game_x.persistence.Config.Rewards;

public sealed class RewardConfigConfig : IEntityTypeConfiguration<RewardPool>
{
    public void Configure(EntityTypeBuilder<RewardPool> b)
    {
        b.ToTable("reward_pools");

        b.HasKey(x => x.Id);

        #region Properties
        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        
        b.Property(g => g.PublicId)
            .HasColumnName("public_id")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");
        
        b.Property(x => x.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(64)
            .HasDefaultValue(RewardPoolType.Roulette);

        b.Property(x => x.TriggerEvents)
            .HasColumnName("trigger_events")
            .HasColumnType("text[]")
            .IsRequired()
            .HasConversion(
                new ValueConverter<UserEventType[], string[]>(
                    v => v.Select(e => e.ToString()).ToArray(),
                    v => v.Select(Enum.Parse<UserEventType>).ToArray()
                )
            )
            .Metadata.SetValueComparer(
                new ValueComparer<UserEventType[]>(
                    (left, right) =>
                        left != null &&
                        right != null &&
                        left.SequenceEqual(right),
                    value => value.Aggregate(0, HashCode.Combine),
                    value => value.ToArray()
                )
            );
        
        b.Property(x => x.Code)
            .HasColumnName("code")
            .IsRequired()
            .HasMaxLength(128);
        
        b.Property(x => x.Title)
            .HasColumnName("title")
            .IsRequired()
            .HasMaxLength(2048)
            .HasDefaultValue(string.Empty);

        b.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(4096);
        
        b.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);
        
        b.Property(x => x.SortOrder)
            .HasColumnName("sort_order")
            .IsRequired()
            .HasDefaultValue(0);
        
        b.Property(x => x.Config)
            .HasColumnName("config")
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<RewardPoolConfigData>(
                    v,
                    JsonSerializerOptions.Default
                )!
            )
            .Metadata.SetValueComparer(
                new ValueComparer<RewardPoolConfigData>(
                    (l, r) => l == r,
                    v => v.GetHashCode(),
                    v => JsonSerializer.Deserialize<RewardPoolConfigData>(
                        JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                        JsonSerializerOptions.Default
                    )!
                )
            );
        
        b.Property(x => x.StartAt)
            .HasColumnName("start_at")
            .HasColumnType("timestamp with time zone");
        
        b.Property(x => x.EndAt)
            .HasColumnName("end_at")
            .HasColumnType("timestamp with time zone");
        
        b.Property(x => x.DeletedAt)
            .HasColumnName("deleted_at")
            .HasColumnType("timestamp with time zone");
        #endregion

        #region Indexes
        b.HasIndex(x => x.Code)
            .HasDatabaseName("ix_reward_pools_code")
            .IsUnique();
        b.HasIndex(x => new { x.Type, x.IsActive })
            .HasDatabaseName("ix_reward_pools_type_active");
        b.HasIndex(x => new { x.StartAt, x.EndAt })
            .HasDatabaseName("ix_reward_items_date_range");
        #endregion
        
        b.HasQueryFilter(x => x.DeletedAt == null);
    }
}