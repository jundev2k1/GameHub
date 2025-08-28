using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.Dtos;
using game_x.application.Features.Accounts.Mapping;
using UserEntity = game_x.domain.Entities.User;

namespace game_x.application.Features.Accounts.Admin.Queries.GetCsCriteriaByAdmin;

public sealed class GetCsCriteriaByAdminHandler(
    ICriteriaBuilder<UserEntity> builder,
    IUserRepo userRepo) : IQueryHandler<GetCsCriteriaByAdminQuery, PaginationResult<AdminDto>>
{
    public async Task<PaginationResult<AdminDto>> Handle(GetCsCriteriaByAdminQuery request, CancellationToken ct = default)
    {
        var items = await userRepo.GetCsAdminByCriteria(
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword =>
                    user => user.UserName!.StartsWith(keyword)),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);
        var result = items.ToSearchResult();
        return result;
    }
}
