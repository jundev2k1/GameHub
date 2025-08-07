namespace game_x.application.Events.OnTransactionCreated;

public record OnTransactionCreatedEvent(ChainTransaction Transaction) : IApplicationEvent;