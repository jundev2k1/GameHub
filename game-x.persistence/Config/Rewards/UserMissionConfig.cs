using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config.Rewards;

public sealed class UserMissionConfig : IEntityTypeConfiguration<UserMission>
{
    public void Configure(EntityTypeBuilder<UserMission> b)
    {
        b.ToTable("user_missions");

        b.HasKey(x => x.Id);

        #region Properties
        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        
        b.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasMaxLength(36);

        b.Property(x => x.MissionId)
            .HasColumnName("mission_id");

        b.Property(x => x.Progress)
            .HasColumnName("progress")
            .HasDefaultValue(0);
        
        b.Property(x => x.Streak)
            .HasColumnName("streak")
            .HasDefaultValue(0);
        
        b.Property(x => x.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(UserMissionStatus.InProgress);
        
        b.Property(x => x.CompletedAt)
            .HasColumnName("completed_at")
            .HasColumnType("timestamp with time zone");

        b.Property(x => x.ClaimedAt)
            .HasColumnName("claimed_at")
            .HasColumnType("timestamp with time zone");

        b.Property(x => x.ResetAt)
            .HasColumnName("reset_at")
            .HasColumnType("timestamp with time zone");
        
        b.Property(x => x.LastProgressAt)
            .HasColumnName("last_progress_at")
            .HasColumnType("timestamp with time zone");
        #endregion

        #region Indexes
        b.HasIndex(x => new { x.UserId, x.MissionId })
            .IsUnique()
            .HasDatabaseName("ix_user_missions_user_id_mission_id");
        
        b.HasIndex(x => new { x.UserId, x.Status })
            .HasDatabaseName("ix_user_missions_user_status");

        b.HasIndex(x => new { x.MissionId, x.Status })
            .HasDatabaseName("ix_user_missions_mission_status");
        #endregion

        #region Relationship
        b.HasOne(x => x.User)
            .WithMany(x => x.UserMissions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Mission)
            .WithMany(x => x.UserMissions)
            .HasForeignKey(x => x.MissionId)
            .OnDelete(DeleteBehavior.Cascade);
        #endregion
    }
}