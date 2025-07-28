namespace game_x.application.Features.Auth.Client.Commands.Register.Client;

public record RegistUserCommand(string Email, string Password, string Nickname) : ICommand<RegistUserResult>;

public record RegistUserResult(string UserId);
