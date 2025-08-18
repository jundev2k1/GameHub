namespace game_x.application.Events.OnUserBalanceChanged;

public record OnUserBalanceChangedEvent(UserBalance UserBalance) : IApplicationEvent;