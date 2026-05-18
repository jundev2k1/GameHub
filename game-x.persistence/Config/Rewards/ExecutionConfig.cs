using game_x.domain.Entities.Rewards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config.Rewards;

public sealed class ExecutionConfig : IEntityTypeConfiguration<Execution>
{
    public void Configure(EntityTypeBuilder<Execution> b)
    {
        b.ToTable("executions");

        b.HasKey(x => x.Id);

        #region Properties
        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        b.Property(x => x.PublicId)
            .HasColumnName("public_id")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired()
            .HasMaxLength(36);

        b.Property(x => x.RewardPoolId)
            .HasColumnName("reward_pool_id")
            .IsRequired(false);

        b.Property(x => x.MissionId)
            .HasColumnName("mission_id")
            .IsRequired(false);

        b.Property(x => x.IdempotencyKey)
            .HasColumnName("idempotency_key")
            .HasMaxLength(256)
            .IsRequired(false);

        b.Property(x => x.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        b.Property(x => x.ResultMetadata)
            .HasColumnName("result_metadata")
            .HasColumnType("jsonb")
            .IsRequired(false);

        b.Property(x => x.ErrorMessage)
            .HasColumnName("error_message")
            .HasMaxLength(2048)
            .IsRequired(false);
        #endregion

        #region Indexes
        b.HasIndex(x => x.PublicId)
            .IsUnique()
            .HasDatabaseName("ux_executions_public_id");

        b.HasIndex(x => new { x.UserId, x.Type })
            .HasDatabaseName("ix_executions_user_type");

        b.HasIndex(x => x.Status)
            .HasDatabaseName("ix_executions_status");

        b.HasIndex(x => x.RewardPoolId)
            .HasDatabaseName("ix_executions_reward_pool");

        b.HasIndex(x => x.MissionId)
            .HasDatabaseName("ix_executions_mission");

        b.HasIndex(x => x.IdempotencyKey)
            .HasDatabaseName("ix_executions_idempotency");
        #endregion

        #region Relationships
        b.HasOne(x => x.User)
            .WithMany(x => x.Executions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.RewardPool)
            .WithMany(x => x.Executions)
            .HasForeignKey(x => x.RewardPoolId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Mission)
            .WithMany()
            .HasForeignKey(x => x.MissionId)
            .OnDelete(DeleteBehavior.Restrict);
        #endregion
    }
}