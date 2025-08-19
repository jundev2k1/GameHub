using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

namespace game_x.application.Features.Chat.Commands.SendSupportMessage;

public sealed record SendSupportMessageCommand(
    string Text,
    string? ClientLocalId = null
) : IRequest<SendSupportMessageResult>;

public sealed record SendSupportMessageResult(
    MessageDto Message,
    ConversationDto? CreatedConversation
);