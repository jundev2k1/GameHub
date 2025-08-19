namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public record MessageDeletedDto(
    int ConversationId, 
    int MessageId);