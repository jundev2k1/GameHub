using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Features.ChainTransactions.Client.Queries.GetMyTransactionDetail;

public record GetMyTransactionDetailQuery(Guid TransactionId) : IQuery<ChainTransactionDetailDto>;