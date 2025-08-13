using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.ChainTransactions.Dtos;
using game_x.application.Features.ChainTransactions.Mapping;

namespace game_x.application.Features.ChainTransactions.Client.Queries.GetOngoingTransactionCriteriaByClient;

public sealed class GetOngoingTransactionCriteriaByClientHandler(
    ICriteriaBuilder<ChainTransaction> builder, 
    IChainTransactionRepo chainTransactionRepo,
    IUserAccessor userAccessor)
    : IQueryHandler<GetOngoingTransactionCriteriaByClientQuery, PaginationResult<ChainTransactionDto>>
{
    public async Task<PaginationResult<ChainTransactionDto>> Handle(GetOngoingTransactionCriteriaByClientQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var items = await chainTransactionRepo.GetOngoingTransactionCriteriaByUserAsync(
            userId,
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);
        
        var result = items.ToSearchResult();
        return result;
    }
}