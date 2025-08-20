using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Features.ChainTransactions.Client.Queries.GetMyTransactionDetail;

public sealed class GetMyTransactionDetailHandler(
    IUserAccessor userAccessor,
    IChainTransactionRepo chainTransactionRepo)
    : IQueryHandler<GetMyTransactionDetailQuery, ChainTransactionDetailDto>
{
    public async Task<ChainTransactionDetailDto> Handle(GetMyTransactionDetailQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var result = await chainTransactionRepo.GetByIdAndUserIdAsync(userId, request.TransactionId, ct);
        return result.Adapt<ChainTransactionDetailDto>();
    }
}