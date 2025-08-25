using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Extensions.FilterExtensions;
using game_x.application.Features.Games.Dtos;
using game_x.application.Features.Games.Mapping;

namespace game_x.application.Features.Games.Client.Queries.GetGameTransactions;

public sealed class GetGameTransactionsHandler(
    ICriteriaBuilder<GameTransaction> builder,
    IGameTransactionRepo gameTransactionRepo)
    : IQueryHandler<GetGameTransactionsQuery, PaginationResult<GameTransactionDto>>
{
    public async Task<PaginationResult<GameTransactionDto>> Handle(GetGameTransactionsQuery request, CancellationToken ct = default)
    {

        var items = await gameTransactionRepo.GetTransactionByCriteriaAsync(
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                options: GameTransactionFilterExtensions.Options),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);

        var result = items.ToSearchResult();
        return result;
    }
}
