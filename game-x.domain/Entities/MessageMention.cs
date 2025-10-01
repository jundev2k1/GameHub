using System.ComponentModel.DataAnnotations;

namespace game_x.domain.Entities;

public sealed class MessageMention : BaseEntity<int>
{
    public int MessageId { get; set; }
    public Message Message { get; set; } = null!;
    [MaxLength(64)]
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
    public MentionKind Kind { get; set; }
    [MaxLength(4096)] 
    public string? Display { get; set; }
    /// <summary>when fan-out delivered</summary>
    public DateTime? DeliveredAt { get; set; }
    /// <summary>When user read after open</summary>
    public DateTime? ReadAt { get; set; }
    
    public static MessageMention Create(
        Message msg,
        string userId,
        MentionKind kind,
        string? display = null,
        DateTime? deliveredAt = null,
        DateTime? readAt = null
    )
    {
        var msgMention = new MessageMention
        {
            Message = msg,
            UserId = userId,
            Kind = kind,
            Display = display,
            DeliveredAt = deliveredAt,
            ReadAt = readAt,
        };
        return msgMention;
    }
}