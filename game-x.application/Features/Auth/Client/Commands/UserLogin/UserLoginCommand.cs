namespace game_x.application.Features.Auth.Client.Commands.UserLogin;

public record UserLoginCommand(string Email, string Password) : ICommand<UserLoginResult>;

public record UserLoginResult(
    string Email,
    string UserId,
    string Nickname,
    string Token,
    string RefreshToken);
