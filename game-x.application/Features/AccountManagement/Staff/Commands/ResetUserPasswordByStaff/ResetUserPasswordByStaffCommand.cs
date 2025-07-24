namespace game_x.application.Features.AccountManagement.Staff.Commands.ResetUserPasswordByStaff;

public record ResetUserPasswordByStaffCommand(string Email, string VerificationCode, string NewPassword) : ICommand;
