using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.ChainTransactions.Dtos;
using game_x.application.Features.ChainTransactions.Mapping;

namespace game_x.application.Features.ChainTransactions.Admin.Queries.GetTransactionCriteriaByAdmin;

public sealed class GetTransactionCriteriaByAdminHandler(
    ICriteriaBuilder<ChainTransaction> builder, 
    IChainTransactionRepo chainTransactionRepo)
    : IQueryHandler<GetTransactionCriteriaByAdminQuery, PaginationResult<ChainTransactionDto>>
{
    public async Task<PaginationResult<ChainTransactionDto>> Handle(GetTransactionCriteriaByAdminQuery request, CancellationToken ct = default)
    {
        var items = await chainTransactionRepo.GetTransactionByCriteriaAsync(
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