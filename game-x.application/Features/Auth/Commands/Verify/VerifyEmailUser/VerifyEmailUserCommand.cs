namespace game_x.application.Features.Auth.Commands.Verify.VerifyEmailUser;

public record VerifyEmailUserCommand(
    string Email,
    string Code) : ICommand;
