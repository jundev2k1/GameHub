using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class SocialLinkConfig : IEntityTypeConfiguration<SocialLink>
{
    public void Configure(EntityTypeBuilder<SocialLink> builder)
    {
        builder.ToTable("social_links");
        builder.HasKey(x => x.Id);

        // 1 pair (UserIdMin, UserIdMax) only 1 record per Kind
        builder.HasIndex(x => new { x.UserIdMin, x.UserIdMax, x.Kind }).IsUnique();
        // Quickly check 'Blocked?' in the direction (Blocker → Blocked)
        builder.HasIndex(x => new { x.Kind, x.BlockerUserId, x.BlockedUserId });
        // Queue of friend requests coming to me
        builder.HasIndex(x => new { x.Kind, x.State, x.AddresseeUserId });
        // List of your accepted (supports searching by any clue)
        builder.HasIndex(x => new { x.Kind, x.State, x.UserIdMin });
        builder.HasIndex(x => new { x.Kind, x.State, x.UserIdMax });
        
        // Concurrency (Postgres xmin)
        builder.Property<uint>("xmin")
            .HasColumnName("xmin").HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();
        
        // Properties
        builder.Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();
        
        builder.Property(al => al.PublicId)
            .HasColumnName("public_id")
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()");
 
        builder.Property(x => x.UserIdMin)
            .HasColumnName("user_id_min")
            .IsRequired();
        
        builder.Property(x => x.UserIdMax)
            .HasColumnName("user_id_max")
            .IsRequired();
        
        builder.Property(x => x.Kind)
            .HasColumnName("kind")
            .IsRequired();
        
        builder.Property(x => x.State)
            .HasColumnName("state")
            .IsRequired();
        
        builder.Property(x => x.RequesterUserId)
            .HasColumnName("requester_user_id")
            .IsRequired(false);
        
        builder.Property(x => x.AddresseeUserId)
            .HasColumnName("addressee_user_id")
            .IsRequired(false);
        
        builder.Property(x => x.BlockerUserId)
            .HasColumnName("blocker_user_id")
            .IsRequired(false);
        
        builder.Property(x => x.BlockedUserId)
            .HasColumnName("blocked_user_id")
            .IsRequired(false);
        
        builder.Property(x => x.RespondedAt)
            .HasColumnName("responded_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
        
        // Relationships
        builder.HasOne(x => x.RequesterUser)
            .WithMany(x => x.RequestedLinks)
            .HasForeignKey(x => x.RequesterUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AddresseeUser)
            .WithMany(x => x.ReceivedRequests)
            .HasForeignKey(x => x.AddresseeUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.BlockerUser)
            .WithMany(x => x.BlocksByMe)
            .HasForeignKey(x => x.BlockerUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.BlockedUser)
            .WithMany(x => x.BlocksToMe)
            .HasForeignKey(x => x.BlockedUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}