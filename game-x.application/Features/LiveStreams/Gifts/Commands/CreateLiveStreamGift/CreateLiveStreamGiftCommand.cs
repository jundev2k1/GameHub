namespace game_x.application.Features.LiveStreams.Gifts.Commands.CreateLiveStreamGift;

public record CreateLiveStreamGiftCommand(
    string Name,
    string? Notes,
    int Priority) : ICommand;
