using System.ComponentModel.DataAnnotations;

namespace game_x.domain.Entities;

public sealed class Message: BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; set; }
    public int ConversationId { get; set; }
    public Conversation Conversation { get; set; } =  null!;

    [MaxLength(64)] 
    public string SenderUserId { get; set; } = null!;
    public User SenderUser { get; set; } = null!;
    public RoleInConversation SenderRole { get; set; } = RoleInConversation.Member;

    public MessageKind Kind { get; set; } = MessageKind.Text; // Determines how clients render and what fields are relevant
    [MaxLength(4096)] 
    public string? Text { get; set; } // Message text when Kind=Text
    public string? PayloadJson { get; set; } // Structured extras (quick replies, buttons, templates, system payloads)
    
    public int? ReplyToMessageId { get; set; } // Support “reply/quote” threads
    public Message? ReplyToMessage { get; set; }

    public bool IsTombstone { get; set; } // True when revoked/removed for everyone. Clients render a “message removed” stub
    
    public DateTime SentAt { get; set; } = DateTime.UtcNow; // Timeline ordering

    // Edit info
    public DateTime? EditedAt { get; set; }
    public int EditCount { get; set; }
    public int CurrentVersion { get; set; } = 1;
    
    public Dictionary<string, HashSet<string>> Reactions { get; set; } = new(StringComparer.Ordinal);
    
    public List<MessageAttachment> Attachments { get; set; } = new();
    public List<MessageEditSnapshot> EditHistory { get; set; } = new();
}