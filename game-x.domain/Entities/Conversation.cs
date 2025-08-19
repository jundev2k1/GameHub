using System.ComponentModel.DataAnnotations;

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
    [MaxLength(64)] 
    public string? CustomerId { get; set; } // the customer
    public User? Customer { get; set; }
    [MaxLength(64)] 
    public string? AssignedAgentId { get; set; } // enables queues/claiming; The agent who owns the support conversation
    public User? AssignedAgent { get; set; }

    public DateTime LastMessageAt { get; set; } = DateTime.UtcNow; // powers list ordering; Updated on every message; used to order inbox/queues
    
    public ICollection<ConversationMember> Members { get; set; } = new List<ConversationMember>(); // Linked users (for both Support & Direct)
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    
    public static Conversation Create(
        ConversationType type,
        string senderUserId
    )
    {
        var conv = new Conversation
        {
            Type = type,
            Status = ConversationStatus.Open,
            CustomerId = senderUserId,
            LastMessageAt = DateTime.UtcNow
        };
        return conv;
    }
}