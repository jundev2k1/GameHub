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
    public bool? IsHidden { get; set; }
    
    // Read pointers
    public int? LastReadMessageId { get; set; }
    public Message? LastReadMessage { get; set; }
    
    /// <summary>Mark the last time the conversation was seen</summary>
    public DateTime? LastSeenAt { get; set; }
    
    /// <summary>The last time the conversation was opened</summary>
    public DateTime? LastDeliveredAt { get; set; }
    
    public static ConversationMember Create(
        Conversation conv,
        string userId,
        RoleInConversation role,
        DateTime? lastDeliveredAt = null
    )
    {
        var convMember = new ConversationMember
        {
            Conversation = conv,
            UserId = userId,
            Role = role,
            JoinedAt = DateTime.UtcNow,
            LastDeliveredAt = lastDeliveredAt,
        };
        return convMember;
    }
}