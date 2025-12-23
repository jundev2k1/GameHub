using game_x.application.Features.Transactions.Dtos;

namespace game_x.application.Features.Transactions.Client.Queries.GetMyTransactionDetail;

public record GetMyTransactionDetailQuery(Guid TransactionId) : IQuery<TransactionInternalDetailDto>;