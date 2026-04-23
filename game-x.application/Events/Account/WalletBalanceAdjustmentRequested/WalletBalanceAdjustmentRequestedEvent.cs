namespace game_x.application.Events.Account.WalletBalanceAdjustmentRequested;

public record WalletBalanceAdjustmentRequestedEvent(string UserId, Guid PlatformId) : IApplicationEvent;
