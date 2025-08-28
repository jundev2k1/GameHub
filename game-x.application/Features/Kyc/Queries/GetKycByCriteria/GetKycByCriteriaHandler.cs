using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.Mapping;
using game_x.application.Features.Kyc.Dtos;
using game_x.application.Features.Kyc.Mapping;

namespace game_x.application.Features.Kyc.Queries.GetKycByCriteria;

public sealed class GetKycByCriteriaHandler(
    ICriteriaBuilder<UserKyc> builder,
    IUserRepo userRepo) : IQueryHandler<GetKycByCriteriaQuery, PaginationResult<UserKycListItemDto>>
{
    public async Task<PaginationResult<UserKycListItemDto>> Handle(GetKycByCriteriaQuery request, CancellationToken ct = default)
    {
        var items = await userRepo.GetUserKycByCriteria(
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword =>
                    kyc => kyc.FullName.StartsWith(keyword)
                        || kyc.User.Email!.StartsWith(keyword)
                        || kyc.User.Nickname.StartsWith(keyword)),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);
        var result = items.ToSearchResult();
        return result;
    }
}
