namespace game_x.application.Features.AccountManagement.User.Commands.GenerateSelfUserQrCode;

public record GenerateSelfUserQrCodeCommand : ICommand<GenerateQrCodeResult>;

public record GenerateQrCodeResult(string QrCode, string ExpiresAt);
