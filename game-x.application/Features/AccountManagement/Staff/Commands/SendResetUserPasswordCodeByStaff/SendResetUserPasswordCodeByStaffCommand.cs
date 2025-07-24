namespace game_x.application.Features.AccountManagement.Staff.Commands.SendResetUserPasswordCodeByStaff;

public record SendResetUserPasswordCodeByStaffCommand(string Email) : ICommand;
