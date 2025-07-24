namespace game_x.application.Events.OnOrderCreated;

public record OnOrderCreatedEvent(Order Order) : IApplicationEvent;