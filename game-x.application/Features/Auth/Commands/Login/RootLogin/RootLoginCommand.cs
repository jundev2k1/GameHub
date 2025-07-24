namespace game_x.application.Features.Auth.Commands.Login.RootLogin;

public record RootLoginCommand(string UserName, string Password) : ICommand<RootLoginResult>;

public record RootLoginResult(string Token);
