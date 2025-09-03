using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Extensions.FilterExtensions;
using game_x.application.Features.BankAccountVerifications.Dtos;
using game_x.application.Features.BankAccountVerifications.Mapping;

namespace game_x.application.Features.BankAccountVerifications.Queries.GetBankAccountByCriteria;

public sealed class GetBankAccountByCriteriaHandler(
    ICriteriaBuilder<UserBankAccount> builder,
    IUserRepo userRepo) : IQueryHandler<GetBankAccountByCriteriaQuery, PaginationResult<BankAccountListItemDto>>
{
    public async Task<PaginationResult<BankAccountListItemDto>> Handle(GetBankAccountByCriteriaQuery request, CancellationToken ct = default)
    {
        var items = await userRepo.GetUserBankAccountByCriteria(
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword =>
                    ba => ba.BankName.ToLower().StartsWith(keyword.ToLower())
                        || ba.BankCode.ToLower().StartsWith(keyword.ToLower())
                        || ba.AccountName.ToLower().StartsWith(keyword.ToLower())
                        || ba.AccountNumber.ToLower().StartsWith(keyword.ToLower()),
                BankAccountFilterExtensions.Options),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);
        var result = items.ToSearchResult();
        return result;
    }
}
