using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Extensions;
using game_x.application.Features.Friends.Dtos;

namespace game_x.application.Features.Friends.Queries.GetFriendRequests;

public sealed class GetFriendRequestsHandler(
    IUserAccessor userAccessor,
    ISocialLinkRepo socialLinkRepo, 
    ICriteriaBuilder<SocialLinkDto> builder): IQueryHandler<GetFriendRequestsQuery, PaginationResult<FriendRequestDto>>
{
    public async Task<PaginationResult<FriendRequestDto>> Handle(GetFriendRequestsQuery request, CancellationToken ct)
    {
        var userId = userAccessor.GetUserId();
        var items = await socialLinkRepo.GetRequestsByCriteriaAsync(
            addresseeUserId: userId,
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword =>
                    link => link.RequesterNickname!.Contains(keyword)),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);

        return items.Transform(items.Items.Adapt<IEnumerable<FriendRequestDto>>()); 
    }
}