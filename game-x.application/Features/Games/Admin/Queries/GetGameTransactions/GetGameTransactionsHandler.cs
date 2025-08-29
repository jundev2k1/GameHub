using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Extensions.FilterExtensions;
using game_x.application.Features.Games.Dtos;
using game_x.application.Features.Games.Mapping;

namespace game_x.application.Features.Games.Admin.Queries.GetGameTransactions;

public sealed class GetGameTransactionsHandler(
    ICriteriaBuilder<Transaction> builder,
    ITransactionRepo transactionRepo)
    : IQueryHandler<GetGameTransactionsQuery, PaginationResult<ListTransactionExternalDto>>
{
    public async Task<PaginationResult<ListTransactionExternalDto>> Handle(GetGameTransactionsQuery request, CancellationToken ct = default)
    {
        var items = await transactionRepo.GetExternalTransactionsAsync(
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
