namespace game_x.application.Features.Auth.Client.Commands.ResendCodeUser;

public record ResendCodeUserCommand(string? Email, string Purpose) : ICommand;
