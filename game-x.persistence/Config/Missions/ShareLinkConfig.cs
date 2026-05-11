using game_x.domain.Entities.Missions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config.Missions;

public sealed class ShareLinkConfig : IEntityTypeConfiguration<ShareLink>
{
    public void Configure(EntityTypeBuilder<ShareLink> b)
    {
        b.ToTable("share_links");

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

        b.Property(x => x.MissionId)
            .HasColumnName("mission_id")
            .IsRequired();

        b.Property(x => x.Code)
            .HasColumnName("code")
            .IsRequired()
            .HasMaxLength(128);

        b.Property(x => x.ClickCount)
            .HasColumnName("click_count")
            .IsRequired()
            .HasDefaultValue(0);

        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        b.Property(x => x.CompletedAt)
            .HasColumnName("completed_at")
            .HasColumnType("timestamp with time zone");

        b.Property(x => x.ExpiredAt)
            .HasColumnName("expired_at")
            .HasColumnType("timestamp with time zone");
        #endregion

        #region Indexes
        b.HasIndex(x => x.PublicId)
            .IsUnique()
            .HasDatabaseName("ux_share_links_public_id");

        b.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("ux_share_links_code");

        b.HasIndex(x => new { x.UserId, x.MissionId })
            .HasDatabaseName("ix_share_links_user_mission");

        b.HasIndex(x => x.Status)
            .HasDatabaseName("ix_share_links_status");

        b.HasIndex(x => x.ExpiredAt)
            .HasDatabaseName("ix_share_links_expired");
        #endregion

        #region Relationships
        b.HasOne(x => x.User)
            .WithMany(x => x.ShareLinks)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Mission)
            .WithMany()
            .HasForeignKey(x => x.MissionId)
            .OnDelete(DeleteBehavior.Cascade);
        #endregion
    }
}