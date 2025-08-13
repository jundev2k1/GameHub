using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Features.ChainTransactions.Client.Queries.GetOngoingTransactionDetailByClient;

public record GetOngoingTransactionDetailByClientQuery(Guid TransactionId) : IQuery<ChainTransactionDetailDto>;