namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public record AttachmentProgressDto(
    int Id, 
    int ConversationId, 
    int MessageId, 
    long BytesSent, 
    long BytesTotal);