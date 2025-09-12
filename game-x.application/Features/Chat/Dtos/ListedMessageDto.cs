namespace game_x.application.Features.Chat.Dtos;

public record ListedMessageDto(
    Guid Id,
    Guid ConversationId,
    string SenderActorId,
    MessageKind Kind,
    RoleInConversation SenderRole,
    string? Text,
    Guid? ReplyToMessageId, 
    bool IsTombstone, 
    DateTime SentAt, 
    DateTime? EditedAt, 
    int EditCount, 
    int CurrentVersion,
    IReadOnlyList<ListedMessageAttachmentDto> Attachments);
    
public record ListedMessageAttachmentDto(
    int SortOrder,
    string BindingStatus,
    string? FileName,
    string? Url);