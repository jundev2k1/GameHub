namespace game_x.application.Features.AccountManagement.Staff.Commands.SendVerificationCode;

public record SendVerificationCodeCommand(string Email) : ICommand;
