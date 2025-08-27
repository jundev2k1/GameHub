using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

namespace game_x.application.Features.Chat.Commands.SendMessageToCustomer;

public sealed record SendMessageToCustomerCommand(
    Guid ConversationId,
    string Text
) : IRequest<SendMessageToCustomerResult>;

public sealed record SendMessageToCustomerResult(
    MessageDto Message,
    ConversationDto Conv
);