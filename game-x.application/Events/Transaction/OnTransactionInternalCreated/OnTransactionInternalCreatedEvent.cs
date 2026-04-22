using game_x.application.Features.Transactions.Dtos;

namespace game_x.application.Events.OnTransactionInternalCreated;

public record OnTransactionInternalCreatedEvent(TransactionInternalDto Transaction) : IApplicationEvent;