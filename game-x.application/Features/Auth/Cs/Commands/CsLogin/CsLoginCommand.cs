namespace game_x.application.Features.Auth.Cs.Commands.CsLogin;

public record CsLoginCommand(string UserName, string Password) : ICommand<CsLoginResult>;

public record CsLoginResult(string UserId, string Username, string AccessToken, string RefreshToken);
