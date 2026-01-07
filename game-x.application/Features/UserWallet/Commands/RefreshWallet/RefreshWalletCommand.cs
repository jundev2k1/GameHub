namespace game_x.application.Features.UserWallet.Commands.RefreshWallet;

public record RefreshWalletCommand(Guid GamePlatformId) : ICommand;
