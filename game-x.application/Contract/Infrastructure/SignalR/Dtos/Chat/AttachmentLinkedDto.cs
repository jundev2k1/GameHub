namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public record AttachmentLinkedDto(
    int Id, 
    int ConversationId, 
    int MessageId, 
    int MediaFileId, 
    int SortOrder);