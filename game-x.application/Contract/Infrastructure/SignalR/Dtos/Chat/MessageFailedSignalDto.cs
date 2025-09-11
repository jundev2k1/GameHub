namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public record MessageFailedSignalDto(string ClientLocalId, Guid? ConversationId = null);