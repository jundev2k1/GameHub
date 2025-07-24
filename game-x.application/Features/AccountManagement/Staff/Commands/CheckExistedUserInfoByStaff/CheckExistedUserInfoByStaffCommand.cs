namespace game_x.application.Features.AccountManagement.Staff.Commands.CheckExistedUserInfoByStaff;

public record CheckExistedUserInfoByStaffCommand(string Email, string PhoneNumber) : ICommand<CheckExistedUserInfoByStaffResult>;

public record CheckExistedUserInfoByStaffResult(bool IsExistedEmail, bool IsExistedPhoneNumber);
