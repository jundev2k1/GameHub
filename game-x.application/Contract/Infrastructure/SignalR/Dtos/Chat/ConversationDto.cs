namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public record ConversationDto(
    string Id, 
    ConversationType Type, 
    ConversationStatus Status, 
    string? AssignedAgentId, 
    string? CustomerId, 
    DateTime LastMessageAt);