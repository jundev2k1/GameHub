namespace game_x.application.Features.AccountManagement.Staff.Commands.ValidateUserQrCode;

public record ValidateUserQrCodeCommand(string QrCode): ICommand<ValidateQrCodeResult>;

public record ValidateQrCodeResult(string UserId, string UserName, string Email, string PhoneNumber);
