namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public record MessageFailedDto(string ClientLocalId, Guid? ConversationId = null);