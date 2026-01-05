using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.TalentWallets.DTOs;

namespace game_x.application.Features.TalentWallets.Queries.GetTalentWalletTransactions;

public sealed class GetTalentWalletTransactionsHandler(
    ICriteriaBuilder<TalentWalletTransactionDto> criteriaBuilder,
    ITalentWalletRepo talentWalletRepo) : IQueryHandler<GetTalentWalletTransactionsQuery, PaginationResult<TalentWalletTransactionDto>>
{
    public async Task<PaginationResult<TalentWalletTransactionDto>> Handle(GetTalentWalletTransactionsQuery request, CancellationToken ct = default)
    {
        var searchResult = await talentWalletRepo.GetsByCriteriaAsync(
            null,
            query => criteriaBuilder.Apply(
                query,
                request.Filters,
                request.Sorts),
            request.PageIndex,
            request.PageSize,
            ct);
        return searchResult;
    }
}
