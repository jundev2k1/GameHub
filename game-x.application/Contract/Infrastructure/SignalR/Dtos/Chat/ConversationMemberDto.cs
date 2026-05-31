namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public record ConversationMemberDto(
    int ConversationId, 
    string UserId, 
    string Role);