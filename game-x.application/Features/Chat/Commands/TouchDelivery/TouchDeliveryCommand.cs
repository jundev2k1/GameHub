namespace game_x.application.Features.Chat.Commands.TouchDelivery;

public sealed record TouchDeliveryCommand(Guid ConversationId) : IRequest<Unit>;