namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public record ListMessageDto(
    Guid Id,
    Guid ConversationId,
    string SenderActorId,
    MessageKind Kind, 
    string? Text,
    Guid? ReplyToMessageId, 
    bool IsTombstone, 
    DateTime SentAt, 
    DateTime? EditedAt, 
    int EditCount, 
    int CurrentVersion,
    IReadOnlyList<ListMessageAttachmentDto> Attachments);
    
public record ListMessageAttachmentDto(
    int SortOrder,
    string BindingStatus,
    string? FileName,
    string? Url);