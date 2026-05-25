using game_x.domain.Entities.Rewards;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config.Rewards;

public sealed class UserMissionClaimConfig : IEntityTypeConfiguration<UserMissionClaim>
{
    public void Configure(EntityTypeBuilder<UserMissionClaim> b)
    {
        b.ToTable("user_mission_claims");

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
            .HasMaxLength(36)
            .IsRequired();

        b.Property(x => x.UserMissionId)
            .HasColumnName("user_mission_id")
            .IsRequired();

        b.Property(x => x.MissionRewardId)
            .HasColumnName("mission_reward_id")
            .IsRequired();

        b.Property(x => x.ExecutionId)
            .HasColumnName("execution_id")
            .IsRequired(false);
        
        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .IsRequired();

        #endregion

        #region Indexes

        b.HasIndex(x => x.PublicId)
            .IsUnique()
            .HasDatabaseName("ux_user_mission_claims_public_id");

        b.HasIndex(x => new { x.UserId, x.MissionRewardId })
            .IsUnique()
            .HasDatabaseName("ux_user_mission_claims_user_reward");

        b.HasIndex(x => x.UserMissionId)
            .HasDatabaseName("ix_user_mission_claims_user_mission");

        b.HasIndex(x => x.ExecutionId)
            .HasDatabaseName("ix_user_mission_claims_execution");

        #endregion

        #region Relationships
        b.HasOne(x => x.User)
            .WithMany(x => x.UserMissionClaims)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.UserMission)
            .WithMany(x => x.Claims)
            .HasForeignKey(x => x.UserMissionId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.MissionReward)
            .WithMany(x => x.UserMissionClaims)
            .HasForeignKey(x => x.MissionRewardId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Execution)
            .WithMany()
            .HasForeignKey(x => x.ExecutionId)
            .OnDelete(DeleteBehavior.Restrict);
        #endregion
    }
}