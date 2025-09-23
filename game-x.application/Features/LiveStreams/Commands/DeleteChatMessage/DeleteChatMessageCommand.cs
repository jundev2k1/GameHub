namespace game_x.application.Features.LiveStreams.Commands.DeleteChatMessage;

public record DeleteChatMessageCommand(string StreamKey, Guid MessageId) : ICommand;
