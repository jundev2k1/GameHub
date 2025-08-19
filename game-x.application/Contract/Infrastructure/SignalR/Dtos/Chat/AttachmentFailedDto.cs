namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public record AttachmentFailedDto(
    int Id, 
    int ConversationId, 
    int MessageId, 
    int SortOrder, 
    string Error /* code */);