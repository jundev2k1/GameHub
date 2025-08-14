using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Features.ChainTransactions.Admin.Queries.GetTransactionDetailById;

public sealed class GetTransactionDetailByIdHandler(
    IChainTransactionRepo chainTransactionRepo)
    : IQueryHandler<GetTransactionDetailByIdQuery, ChainTransactionDetailDto>
{
    public async Task<ChainTransactionDetailDto> Handle(GetTransactionDetailByIdQuery request, CancellationToken ct = default)
    {
        var result = await chainTransactionRepo.GetByIdAsync(request.TransactionId, ct);
        return result.Adapt<ChainTransactionDetailDto>();
    }
}