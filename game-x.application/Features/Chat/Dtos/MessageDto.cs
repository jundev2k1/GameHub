namespace game_x.application.Features.Chat.Dtos;

public class MessageDto
{
    public int Id { get; set; }
    public string? ClientLocalId { get; set; } = String.Empty;
    public Guid PublicId { get; set; }
    public Guid ConversationId { get; set; }
    public string SenderActorId { get; set; } = null!;
    public User? SenderUser { get; set; }
    public MessageKind Kind { get; set; }
    public RoleInConversation SenderRole { get; set; }
    public string? Text { get; set; }
    public Guid? ReplyToMessageId { get; set; }
    public bool IsTombstone { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? EditedAt { get; set; }
    public int EditCount { get; set; }
    public bool? IsMentionAll { get; set; }
    public IReadOnlyList<DirectMention>? DirectMentions { get; set; }
    public int CurrentVersion { get; set; }
    public List<MessageAttachmentDto> Attachments { get; set; } = new();
};

public class MessageAttachmentDto
{
    public int SortOrder { get; set; }
    public AttachmentBindingStatus BindingStatus { get; set; }
    public MediaFile? Attachment { get; set; }
};