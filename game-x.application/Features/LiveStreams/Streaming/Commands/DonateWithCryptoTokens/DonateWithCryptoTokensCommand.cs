using System.Text.Json.Serialization;

namespace game_x.application.Features.LiveStreams.Streaming.Commands.DonateWithCryptoTokens;

public record DonateWithCryptoTokensCommand(
    [property: JsonIgnore] string? StreamKey,
    decimal Amount,
    string Message,
    Guid CryptoTokenId) : ICommand;
