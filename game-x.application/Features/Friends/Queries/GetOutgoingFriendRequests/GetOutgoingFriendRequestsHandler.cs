using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Extensions;
using game_x.application.Features.Friends.Dtos;

namespace game_x.application.Features.Friends.Queries.GetOutgoingFriendRequests;

public sealed class GetOutgoingFriendRequestsHandler(
    IUserAccessor userAccessor,
    ISocialLinkRepo socialLinkRepo, 
    ICriteriaBuilder<SocialLinkDto> builder): IQueryHandler<GetOutgoingFriendRequestsQuery, PaginationResult<OutgoingFriendRequestDto>>
{
    public async Task<PaginationResult<OutgoingFriendRequestDto>> Handle(GetOutgoingFriendRequestsQuery request, CancellationToken ct)
    {
        var userId = userAccessor.GetUserId();
        var items = await socialLinkRepo.GetOutgoingRequestsByCriteriaAsync(
            requesterUserId: userId,
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword =>
                    link => link.RequesterNickname!.Contains(keyword)),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);

        return items.Transform(items.Items.Adapt<IEnumerable<OutgoingFriendRequestDto>>()); 
    }
}