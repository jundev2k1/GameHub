using game_x.application.Features.Transactions.Dtos;

namespace game_x.application.Features.Transactions.Admin.Queries.GetTransactionDetailById;

public record GetTransactionDetailByIdQuery(Guid TransactionId) : IQuery<TransactionInternalDetailDto>;