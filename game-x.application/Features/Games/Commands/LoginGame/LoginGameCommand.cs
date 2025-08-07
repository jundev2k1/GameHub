namespace game_x.application.Features.Games.Commands.LoginGame;

public record LoginGameCommand(
    string GameCode,
    string Locale,
    string Address,
    string ReturnUrl) : ICommand<LoginGameResult>;

public record LoginGameResult(string EmbededLink);
