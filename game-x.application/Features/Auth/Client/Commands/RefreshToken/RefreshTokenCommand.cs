namespace game_x.application.Features.Auth.Client.Commands.RefreshToken;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : ICommand<RefreshTokenResult>;

public record RefreshTokenResult(string AccessToken, string RefreshToken);
