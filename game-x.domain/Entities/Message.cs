using System.ComponentModel.DataAnnotations;

namespace game_x.domain.Entities;

public sealed class Message: BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; set; }
    public int ConversationId { get; set; }
    public Conversation Conversation { get; set; } =  null!;
    /// <summary>Guest or User</summary>
    [MaxLength(64)]
    public string SenderActorId { get; set; } = null!;
    [MaxLength(64)] 
    public string? SenderUserId { get; set; }
    public User? SenderUser { get; set; }
    public RoleInConversation SenderRole { get; set; } = RoleInConversation.Member;
    /// <summary>Determines how clients render and what fields are relevant</summary>
    public MessageKind Kind { get; set; } = MessageKind.Text;
    [MaxLength(4096)] 
    public string? Text { get; set; }
    /// <summary>Structured extras (quick replies, buttons, templates, system payloads)</summary>
    public string? PayloadJson { get; set; }
    /// <summary>Support “reply/quote” threads</summary>
    public int? ReplyToMessageId { get; set; }
    public Message? ReplyToMessage { get; set; }
    /// <summary>True when revoked/removed for everyone. Clients render a “message removed” stub</summary>
    public bool IsTombstone { get; set; }
    /// <summary>Timeline ordering</summary>
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    /// <summary>Mention all members (in Public Channel)</summary>
    public bool? IsMentionAll { get; set; }
    // Edit info
    public DateTime? EditedAt { get; set; }
    public int EditCount { get; set; }
    public int CurrentVersion { get; set; } = 1;
    
    public Dictionary<string, HashSet<string>> Reactions { get; set; } = new(StringComparer.Ordinal);
    
    public List<MessageAttachment> Attachments { get; set; } = new();
    public List<MessageMention> Mentions { get; set; } = new();
    public List<MessageEditSnapshot> EditHistory { get; set; } = new();
    
    public static Message Create(
        Conversation conv,
        string senderActorId,
        RoleInConversation senderRole,
        MessageKind kind,
        string? text = null,
        string? senderUserId = null,
        int? replyToMessageId = null,
        bool? isMentionAll = null
    )
    {
        var msg = new Message
        {
            Conversation = conv,
            SenderActorId = senderActorId,
            SenderUserId = senderUserId,
            SenderRole = senderRole,
            Kind = kind,
            Text = text?.Trim(),
            SentAt = DateTime.UtcNow,
            IsTombstone = false,
            EditCount = 0,
            CurrentVersion = 1,
            ReplyToMessageId = replyToMessageId,
            IsMentionAll = isMentionAll
        };
        return msg;
    }
}