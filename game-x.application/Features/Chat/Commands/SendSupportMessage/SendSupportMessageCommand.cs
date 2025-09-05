using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

namespace game_x.application.Features.Chat.Commands.SendSupportMessage;

public sealed record SendSupportMessageCommand(
    string Text
) : IRequest<SendSupportMessageResult>;

public sealed record SendSupportMessageResult(
    MessageDto Message,
    ConversationSignalDto Conv
);