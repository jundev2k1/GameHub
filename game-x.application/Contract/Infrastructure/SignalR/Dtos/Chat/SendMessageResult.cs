namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public sealed record SendMessageResult(
    ListMessageDto ListMessage,
    ConversationSignalDto Conv
);