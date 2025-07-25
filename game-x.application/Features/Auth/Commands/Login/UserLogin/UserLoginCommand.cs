namespace game_x.application.Features.Auth.Commands.Login.UserLogin;

public record UserLoginCommand(string UserName, string Password) : ICommand<UserLoginResult>;

public record UserLoginResult(string UserName, string UserId, string Token, DateTime ExpiresAt, string[] Roles);
