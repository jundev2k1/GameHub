namespace game_x.application.Features.Auth.Admin.Commands.AdminLogin;

public record LoginAdminCommand(string UserName, string Password) : ICommand<AdminLoginResult>;

public record AdminLoginResult(string UserName, string UserId, string AccessToken, string RefreshToken);
