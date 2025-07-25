namespace game_x.application.Features.Auth.Commands.Login.CsLogin;

public record CsLoginCommand(string UserName, string Password) : ICommand<CsLoginResult>;

public record CsLoginResult(string UserName, string UserId, string Token, DateTime ExpiresAt, string[] Roles);
