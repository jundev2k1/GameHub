namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public record TypingDto(
    int ConversationId, 
    string UserId, 
    bool IsTyping);