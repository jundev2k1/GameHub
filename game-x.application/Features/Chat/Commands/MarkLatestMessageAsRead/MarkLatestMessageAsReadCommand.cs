namespace game_x.application.Features.Chat.Commands.MarkLatestMessageAsRead;

public sealed record MarkLatestMessageAsReadCommand(Guid ConversationId) : ICommand<Guid>;
