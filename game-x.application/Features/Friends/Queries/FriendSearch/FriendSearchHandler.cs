using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Extensions;
using game_x.application.Features.Accounts.Dtos;
using game_x.application.Features.Friends.Dtos;


namespace game_x.application.Features.Friends.Queries.FriendSearch;

public class FriendSearchHandler(IUserRepo userRepo, ICriteriaBuilder<UserDto> builder): IQueryHandler<FriendSearchQuery, PaginationResult<FriendSearchResultDto>>
{
    public async Task<PaginationResult<FriendSearchResultDto>> Handle(FriendSearchQuery request, CancellationToken ct)
    {
        var items = await userRepo.GetUserByCriteriaAsync(
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword =>
                    user =>
                        (user.Nickname.Contains(keyword)) ||
                        (user.UserName.Contains(keyword)) ||
                        (user.Email.Contains(keyword))),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);

        return items.Transform(items.Items.Adapt<IEnumerable<FriendSearchResultDto>>()); 
    }
}