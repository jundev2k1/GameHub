namespace game_x.application.Features.Chat.Commands.DeleteMessage;

public sealed record DeleteMessageCommand(Guid ConversationId, Guid MessageId) : ICommand;