namespace game_x.application.Events.OnUserBalanceChanged.FromUxm;

public record OnUserBalanceChangedFromUxmEvent(string? OrderNumber) : IApplicationEvent;