namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public record MessageSignalDto(
    Guid Id,
    string ClientLocalId,
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
    IReadOnlyList<MessageAttachmentSignalDto> Attachments);
    
public record MessageAttachmentSignalDto(
    int SortOrder,
    string BindingStatus,
    string? FileName,
    string? Url);