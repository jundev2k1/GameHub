namespace game_x.application.Features.LiveStreams.Streaming.Commands.DeleteChatMessage;

public record DeleteChatMessageCommand(string StreamKey, Guid MessageId) : ICommand;
