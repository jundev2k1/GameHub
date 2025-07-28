namespace game_x.application.Features.Auth.Client.Commands.VerifyEmailUser;

public record VerifyEmailUserCommand(
    string Email,
    string Code) : ICommand;
