namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public record InboxSignalDto(
    Guid ConversationId,
    string Preview,
    DateTime At);