namespace game_x.application.Events.OnWithdrawalOrderCreated;

public record OnWithdrawalOrderCreatedEvent(string Email) : IApplicationEvent;