namespace game_x.application.Events.Transaction.OnTransactionUpdated;

public record OnTransactionUpdatedEvent(Guid TransactionId) : IApplicationEvent;
