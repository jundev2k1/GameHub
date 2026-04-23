namespace game_x.application.Events.Transactions.OnTransactionUpdated;

public record OnTransactionUpdatedEvent(Guid TransactionId) : IApplicationEvent;
