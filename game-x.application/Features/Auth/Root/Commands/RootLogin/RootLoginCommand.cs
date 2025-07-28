namespace game_x.application.Features.Auth.Root.Commands.RootLogin;

public record RootLoginCommand(string UserName, string Password) : ICommand<RootLoginResult>;

public record RootLoginResult(string Token);
