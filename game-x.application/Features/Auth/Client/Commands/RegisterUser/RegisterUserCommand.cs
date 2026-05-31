namespace game_x.application.Features.Auth.Client.Commands.RegisterUser;

public record RegisterUserCommand(string Email, string Password, string Nickname) : ICommand<RegisterUserResult>;

public record RegisterUserResult(string UserId);
