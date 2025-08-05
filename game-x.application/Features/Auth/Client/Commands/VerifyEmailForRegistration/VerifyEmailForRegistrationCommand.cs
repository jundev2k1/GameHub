namespace game_x.application.Features.Auth.Client.Commands.VerifyEmailForRegistration;

public record VerifyEmailForRegistrationCommand(
    string Email,
    string Code) : ICommand;
