using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.TalentWallets.DTOs;
using game_x.application.Features.TalentWallets.Mapping;

namespace game_x.application.Features.TalentWallets.Queries.GetMyTalentWalletTransactions;

public sealed class GetMyTalentWalletTransactionsHandler(
    IUserAccessor userAccessor,
    ICriteriaBuilder<TalentWalletTransaction> criteriaBuilder,
    ITalentWalletRepo talentWalletRepo) : IQueryHandler<GetMyTalentWalletTransactionsQuery, PaginationResult<TalentWalletTransactionDto>>
{
    public async Task<PaginationResult<TalentWalletTransactionDto>> Handle(GetMyTalentWalletTransactionsQuery request, CancellationToken ct = default)
    {
        var talentId = userAccessor.GetUserId();
        var searchResult = await talentWalletRepo.GetsByCriteriaAsync(
            query => criteriaBuilder.Apply(
                query.Where(q => q.TalentId == talentId),
                request.Filters,
                request.Sorts),
            request.PageIndex,
            request.PageSize,
            ct);
        return searchResult.ToSearchResult();
    }
}
