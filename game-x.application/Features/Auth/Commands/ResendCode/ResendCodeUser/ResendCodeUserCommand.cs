namespace game_x.application.Features.Auth.Commands.ResendCode.ResendCodeUser;

public record ResendCodeUserCommand(string Email) : ICommand;
