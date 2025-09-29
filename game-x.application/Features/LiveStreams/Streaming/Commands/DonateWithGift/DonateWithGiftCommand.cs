using System.Text.Json.Serialization;

namespace game_x.application.Features.LiveStreams.Streaming.Commands.DonateWithGift;

public record DonateWithGiftCommand(
    [property: JsonIgnore] string? StreamKey,
    Guid GiftId,
    string Message,
    Guid CryptoTokenId) : ICommand;
