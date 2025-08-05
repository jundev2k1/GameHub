namespace game_x.application.Features.Auth.Client.Commands.ResetPasswordUser;

public record ResetPasswordUserCommand(string Token, string Password) : ICommand;
