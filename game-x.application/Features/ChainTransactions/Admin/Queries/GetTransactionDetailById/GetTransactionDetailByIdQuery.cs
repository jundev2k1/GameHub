using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Features.ChainTransactions.Admin.Queries.GetTransactionDetailById;

public record GetTransactionDetailByIdQuery(Guid TransactionId) : IQuery<TransactionInternalDetailDto>;