namespace game_x.application.Features.Auth.Commands.Login.AdminLogin;

public record AdminLoginCommand(string UserName, string Password) : ICommand<AdminLoginResult>;

public record AdminLoginResult(string UserName, string UserId, string Token, DateTime ExpiresAt, string[] Roles);
