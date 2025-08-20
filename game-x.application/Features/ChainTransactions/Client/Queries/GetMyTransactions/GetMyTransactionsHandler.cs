using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Extensions.FilterExtensions;
using game_x.application.Features.ChainTransactions.Dtos;
using game_x.application.Features.ChainTransactions.Mapping;

namespace game_x.application.Features.ChainTransactions.Client.Queries.GetMyTransactions;

public sealed class GetMyTransactionsHandler(
    ICriteriaBuilder<ChainTransaction> builder, 
    IChainTransactionRepo chainTransactionRepo,
    IUserAccessor userAccessor)
    : IQueryHandler<GetMyTransactionsQuery, PaginationResult<ChainTransactionDto>>
{
    public async Task<PaginationResult<ChainTransactionDto>> Handle(GetMyTransactionsQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var items = await chainTransactionRepo.GetMyTransactionsAsync(
            userId,
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                options: ChainTransactionFilterExtensions.Options),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);
        
        var result = items.ToSearchResult();
        return result;
    }
}
