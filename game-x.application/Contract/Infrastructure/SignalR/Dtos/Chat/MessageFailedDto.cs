namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public record MessageFailedDto(
    Guid Id, 
    Guid ConversationId, 
    string SenderUserId, 
    MessageKind Kind, 
    string? Text, 
    Guid? ReplyToMessageId, 
    bool IsTombstone, 
    DateTime SentAt, 
    DateTime? EditedAt, 
    int EditCount, 
    int CurrentVersion);