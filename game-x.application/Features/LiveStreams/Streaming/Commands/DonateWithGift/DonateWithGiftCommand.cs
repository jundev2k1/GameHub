namespace game_x.application.Features.LiveStreams.Streaming.Commands.DonateWithGift;

public record DonateWithGiftCommand(string StreamKey, Guid GiftId, string Message) : ICommand;
