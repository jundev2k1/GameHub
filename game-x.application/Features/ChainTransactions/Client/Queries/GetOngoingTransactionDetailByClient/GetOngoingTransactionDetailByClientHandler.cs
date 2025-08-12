using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Features.ChainTransactions.Client.Queries.GetOngoingTransactionDetailByClient;

public sealed class GetOngoingTransactionDetailByClientHandler(
    IChainTransactionRepo chainTransactionRepo,
    IUserAccessor userAccessor)
    : IQueryHandler<GetOngoingTransactionDetailByClientQuery, ChainTransactionDetailDto>
{
    public async Task<ChainTransactionDetailDto> Handle(GetOngoingTransactionDetailByClientQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var result = await chainTransactionRepo.GetOngoingTransactionDetailByUserAsync(userId, request.TransactionId, ct);
        return result.Adapt<ChainTransactionDetailDto>();
    }
}