using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Events.OnTransactionInternalCreated;

public record OnTransactionInternalCreatedEvent(TransactionInternalDto Transaction) : IApplicationEvent;