namespace game_x.application.Features.LiveStreams.Commands.SendChatMessage;

public record SendChatMessageCommand(string StreamKey, string MessageId, string Message) : ICommand;
