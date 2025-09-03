using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Extensions.FilterExtensions;
using game_x.application.Features.ChainTransactions.Dtos;
using game_x.application.Features.ChainTransactions.Mapping;

namespace game_x.application.Features.ChainTransactions.Client.Queries.GetMyTransactions;

public sealed class GetMyTransactionsHandler(
    ICriteriaBuilder<Transaction> builder, 
    ITransactionRepo transactionRepo,
    IUserAccessor userAccessor)
    : IQueryHandler<GetMyTransactionsQuery, PaginationResult<ListTransactionInternalDto>>
{
    public async Task<PaginationResult<ListTransactionInternalDto>> Handle(GetMyTransactionsQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var items = await transactionRepo.GetMyInternalTransactionsAsync(
            userId,
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                options: TransactionFilterExtensions.Options),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);
        
        var result = items.ToSearchResult();
        return result;
    }
}
