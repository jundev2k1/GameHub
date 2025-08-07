using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Commands.LoginGame;

public record LoginGameCommand(
    string GameCode,
    string Locale,
    string Address,
    string ReturnUrl,
    string Language,
    [property: JsonIgnore]string? IpAddress) : ICommand<LoginGameResult>;

public record LoginGameResult(string EmbededLink);
