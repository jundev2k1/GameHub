using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.Admin.Queries.GetUserCriteriaByAdmin;
using game_x.application.Features.Accounts.Dtos;

namespace game_x.application.Features.AccountManagement.Admin.Queries.GetUserCriteriaByAdmin;

public sealed class GetUserCriteriaByAdminHandler(
    IUserRepo userRepo,
    ICriteriaBuilder<UserListItemDto> builder)
    : IQueryHandler<GetUserCriteriaByAdminQuery, PaginationResult<UserListItemDto>>
{
    public async Task<PaginationResult<UserListItemDto>> Handle(GetUserCriteriaByAdminQuery request,
        CancellationToken ct = default)
    {
        var items = await userRepo.GetUserByCriteriaAsync(
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword =>
                    user => (user.Nickname != null && user.Nickname.Contains(keyword))
                        || (user.UserName != null && user.UserName.Contains(keyword))
                        || (user.Email != null && user.Email.Contains(keyword))),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);

        return items;
    }
}
