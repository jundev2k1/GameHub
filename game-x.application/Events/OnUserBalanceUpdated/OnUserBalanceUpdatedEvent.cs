namespace game_x.application.Events.OnUserBalanceUpdated;

public record OnUserBalanceUpdatedEvent(string UserId) : IApplicationEvent;