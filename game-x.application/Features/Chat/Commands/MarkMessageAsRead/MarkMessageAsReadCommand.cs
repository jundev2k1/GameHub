namespace game_x.application.Features.Chat.Commands.MarkMessageAsRead;

public sealed record MarkMessageAsReadCommand(Guid ConversationId, Guid LastReadMessageId) : ICommand;