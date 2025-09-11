namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public sealed record CreatedMessageSignalResult(
    MessageSignalDto Msg,
    ConversationSignalDto Conv
);