namespace game_x.application.Events.Account.OnUserBalanceUpdated;

public record OnUserBalanceUpdatedEvent(string UserId, Guid? PlatformId = null) : IApplicationEvent;
