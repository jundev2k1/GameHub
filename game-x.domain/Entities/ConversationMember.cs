using System.ComponentModel.DataAnnotations;

namespace game_x.domain.Entities;

/// <summary>
///     Controls authorization and fan-out (who should receive messages). Works for both support and DMs
/// </summary>
public sealed class ConversationMember: BaseEntity<int>, IAuditable
{
    public int ConversationId { get; set; }
    public Conversation Conversation { get; set; } = null!;
    [MaxLength(64)] 
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
    public RoleInConversation Role { get; set; } = RoleInConversation.Member;
    
    // Membership lifecycle
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LeftAt { get; set; }
    
    // Read pointers
    public int? LastReadMessageId { get; set; }
    public Message? LastReadMessage { get; set; }
    
    public DateTime? LastDeliveredAt { get; set; }
    
    public static ConversationMember Create(
        int convId,
        string userId,
        RoleInConversation role
    )
    {
        var convMember = new ConversationMember
        {
            ConversationId = convId,
            UserId = userId,
            Role = role,
            JoinedAt = DateTime.UtcNow
        };
        return convMember;
    }
}