namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public record ReadPointerDto(
    int ConversationId, 
    string UserId, 
    int? LastReadMessageId, 
    DateTime UpdatedAt);