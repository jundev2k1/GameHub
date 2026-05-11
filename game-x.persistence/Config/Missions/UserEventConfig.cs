using game_x.domain.Entities.Missions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config.Missions;

public sealed class UserEventConfig : IEntityTypeConfiguration<UserEvent>
{
    public void Configure(EntityTypeBuilder<UserEvent> b)
    {
        b.ToTable("user_events");

        b.HasKey(x => x.Id);

        #region Properties
        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        
        b.Property(g => g.PublicId)
            .HasColumnName("public_id")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");
        
        b.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired()
            .HasMaxLength(36);

        b.Property(x => x.Type)
            .HasColumnName("type")
            .IsRequired();

        b.Property(x => x.Value)
            .HasColumnName("value")
            .HasPrecision(18, 3);

        b.Property(x => x.RefType)
            .HasColumnName("ref_type")
            .IsRequired(false);

        b.Property(x => x.RefId)
            .HasColumnName("ref_id");

        b.Property(x => x.Metadata)
            .HasColumnName("metadata")
            .HasColumnType("jsonb");
        #endregion

        #region Indexes
        b.HasIndex(x => x.PublicId)
            .IsUnique()
            .HasDatabaseName("ux_user_events_public_id");
        
        b.HasIndex(x => new { x.UserId, x.Type, x.CreatedAt })
            .HasDatabaseName("ix_user_events_user_type_created");
        
        b.HasIndex(x => new { x.Type, x.CreatedAt })
            .HasDatabaseName("ix_user_events_type_created");
        
        b.HasIndex(x => new { x.RefType, x.RefId })
            .HasDatabaseName("ix_user_events_ref");
        #endregion

        #region Relationship
        b.HasOne(x => x.User)
            .WithMany(x => x.UserEvents)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        #endregion
    }
}