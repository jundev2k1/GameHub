using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.TalentWallets.DTOs;
using game_x.application.Features.TalentWallets.Mapping;

namespace game_x.application.Features.TalentWallets.Queries.GetTalentWallets;

public sealed class GetTalentWalletsHandler(
    ICriteriaBuilder<TalentWalletTransaction> criteriaBuilder,
    ITalentWalletRepo talentWalletRepo) : IQueryHandler<GetTalentWalletsQuery, PaginationResult<TalentWalletTransactionDto>>
{
    public async Task<PaginationResult<TalentWalletTransactionDto>> Handle(GetTalentWalletsQuery request, CancellationToken ct = default)
    {
        var searchResult = await talentWalletRepo.GetsByCriteriaAsync(
            query => criteriaBuilder.Apply(
                query,
                request.Filters,
                request.Sorts),
            request.PageIndex,
            request.PageSize,
            ct);
        return searchResult.ToSearchResult();
    }
}
