using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace game_x.persistence.Config;

public sealed class ConversationMemberConfig : IEntityTypeConfiguration<ConversationMember>
{
    public void Configure(EntityTypeBuilder<ConversationMember> builder)
    {
        builder.ToTable("conversation_members");
        builder.HasKey(x => x.Id);
        
        // Uniqueness: one membership per user per conversation
        builder.HasIndex(x => new { x.ConversationId, x.UserId }).IsUnique();
        // Fast lookups: all convos for a user
        builder.HasIndex(x => new { x.UserId, x.ConversationId });
        
        // Properties
        builder.Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ConversationId)
            .HasColumnName("conversation_id")
            .IsRequired();
        
        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();
        
        builder.Property(x => x.Role)
            .HasColumnName("role")
            .IsRequired()
            .HasDefaultValue(RoleInConversation.Member);

        builder.Property(x => x.JoinedAt)
            .HasColumnName("joined_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.LeftAt)
            .HasColumnName("left_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
        
        builder.Property(x => x.LastReadMessageId)
            .HasColumnName("last_read_message_id")
            .IsRequired(false);
        
        builder.Property(x => x.LastDeliveredAt)
            .HasColumnName("last_delivered_at")
            .IsRequired(false);
    }
}