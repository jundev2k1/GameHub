namespace game_x.application.Events.OnOrderCompleted;

public record OnOrderCompletedEvent(Order Order) : IApplicationEvent;