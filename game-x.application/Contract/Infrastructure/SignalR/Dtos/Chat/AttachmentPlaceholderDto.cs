namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public record AttachmentPlaceholderDto(
    int Id, 
    int ConversationId, 
    int MessageId, 
    int SortOrder, 
    string AddedByUserId, 
    string BindingStatus /* Pending */, 
    string? Error /* code */);