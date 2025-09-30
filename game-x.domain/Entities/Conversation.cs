using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace game_x.domain.Entities;

/// <summary>
///     a chat room; one table for both support and DMs keeps all features reusable.
/// </summary>
public sealed class Conversation: BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; set; }
    public ConversationType Type { get; set; }
    public ConversationStatus Status { get; set; } = ConversationStatus.Open;

    // Support-only conveniences
    /// <summary>The customer who creates the conversation for support</summary>
    [MaxLength(64)] 
    public string? CustomerId { get; set; }
    public User? Customer { get; set; }
    /// <summary>The guest who creates the conversation for support</summary>
    [MaxLength(64)]
    
    public string? GuestId { get; set; }
    
    /// <summary>Enables queues/claiming; The agent who owns the support conversation</summary>
    [MaxLength(64)] 
    public string? AssignedAgentId { get; set; }
    public User? AssignedAgent { get; set; }
    /// <summary>Powers list ordering; Updated on every message; used to order inbox/queues</summary>
    public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;
    /// <summary>Linked users (for both Support & Direct)</summary>
    public ICollection<ConversationMember> Members { get; set; } = new List<ConversationMember>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    [NotMapped]
    public int? UnreadCount { get; set; }
    
    public static Conversation Create(
        ConversationType type,
        string? senderUserId = null,
        string? senderGuestId = null
    )
    {
        var conv = new Conversation
        {
            Type = type,
            Status = ConversationStatus.Open,
            CustomerId = senderUserId,
            GuestId = senderGuestId,
            LastMessageAt = DateTime.UtcNow
        };
        return conv;
    }
    
    public void Claim(string agentId)
    {
        Status = ConversationStatus.Claimed;
        AssignedAgentId =  agentId;
    }
}