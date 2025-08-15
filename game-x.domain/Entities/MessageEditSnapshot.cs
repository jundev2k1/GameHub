using System.ComponentModel.DataAnnotations;

namespace game_x.domain.Entities;

public sealed class MessageEditSnapshot: BaseEntity<int>
{
    public int MessageId { get; set; }
    public Message Message { get; set; } = null!;
    public int VersionNumber { get; set; }
    [MaxLength(64)] 
    public string EditorUserId { get; set; } = null!;
    public User EditorUser { get; set; } = null!;
    public DateTime EditedAt { get; set; }
    public MessageKind MessageKind { get; set; }
    [MaxLength(4096)] 
    public string? Text { get; set; }
    public string? PayloadJson { get; set; }
    public int? ReplyToMessageId { get; set; }
    public Message? ReplyToMessage { get; set; }
    public bool IsTombstone { get; set; }
    public List<int>? AttachmentIds { get; set; }
}