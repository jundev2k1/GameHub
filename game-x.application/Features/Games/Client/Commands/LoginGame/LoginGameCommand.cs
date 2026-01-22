using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Client.Commands.LoginGame;

public record LoginGameCommand(
    [property: JsonIgnore] Guid? GamePlatformId,
    string GameCode,
    string Locale,
    string Address,
    string ReturnUrl,
    [property: JsonIgnore] string? IpAddress) : ICommand<LoginGameResult>;

public record LoginGameResult(
    string EmbededLink, 
    string Token,
    string Note);