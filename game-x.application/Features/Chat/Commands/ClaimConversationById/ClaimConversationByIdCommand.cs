namespace game_x.application.Features.Chat.Commands.ClaimConversationById;

public sealed record ClaimConversationByIdCommand(Guid? ConversationId): IRequest<Unit>;