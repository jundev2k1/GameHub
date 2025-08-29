using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Features.ChainTransactions.Admin.Queries.GetTransactionDetailById;

public sealed class GetTransactionDetailByIdHandler(
    ITransactionRepo transactionRepo)
    : IQueryHandler<GetTransactionDetailByIdQuery, TransactionInternalDetailDto>
{
    public async Task<TransactionInternalDetailDto> Handle(GetTransactionDetailByIdQuery request, CancellationToken ct = default)
    {
        var result = await transactionRepo.GetByIdAsync(request.TransactionId, ct);
        return result.Adapt<TransactionInternalDetailDto>();
    }
}