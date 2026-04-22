namespace game_x.application.Events.WalletBalanceAdjustmentRequested;

public record WalletBalanceAdjustmentRequestedEvent(string UserId, Guid PlatformId) : IApplicationEvent;
