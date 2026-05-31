namespace game_x.application.Features.Accounts.User.Commands.GenerateSelfUserQrCode;

public record GenerateSelfUserQrCodeCommand : ICommand<GenerateQrCodeResult>;

public record GenerateQrCodeResult(string QrCode, string ExpiresAt);
