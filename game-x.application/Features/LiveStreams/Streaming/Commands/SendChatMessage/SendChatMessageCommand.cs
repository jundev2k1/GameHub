namespace game_x.application.Features.LiveStreams.Streaming.Commands.SendChatMessage;

public record SendChatMessageCommand(string StreamKey, string MessageId, string Message) : ICommand;
