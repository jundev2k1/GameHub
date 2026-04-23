using game_x.application.Features.Transactions.Dtos;

namespace game_x.application.Events.Transactions.OnTransactionInternalCreated;

public record OnTransactionInternalCreatedEvent(TransactionInternalDto Transaction) : IApplicationEvent;