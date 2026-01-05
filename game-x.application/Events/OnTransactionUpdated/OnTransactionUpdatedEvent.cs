namespace game_x.application.Events.OnTransactionUpdated;

public record OnTransactionUpdatedEvent(Guid TransactionId) : IApplicationEvent;
