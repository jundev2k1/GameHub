using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Extensions;
using game_x.application.Features.Friends.Dtos;

namespace game_x.application.Features.Friends.Queries.GetOutgoingFriendRequests;

public sealed class GetOutgoingFriendRequestsHandler(
    IUserAccessor userAccessor,
    ISocialLinkRepo socialLinkRepo,
    IFileManagerCacheService fileCache,
    ICriteriaBuilder<SocialLink> builder): IQueryHandler<GetOutgoingFriendRequestsQuery, PaginationResult<OutgoingFriendRequestDto>>
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
                    link => link.AddresseeUser!.Nickname.Contains(keyword)),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);

        var dtoItems = await Task.WhenAll(
            items.Items.Select(async m =>
            {
                var avatar = m.AddresseeUser?.Avatar != null ? await fileCache.GetImageUrl(m.AddresseeUser.Avatar, ct) : null;
                var linkDto = m.Adapt<SocialLinkDto>();
                return linkDto.Adapt<OutgoingFriendRequestDto>() with {AddresseeAvatarUrl = avatar?.Url};
            })
        );
        
        return items.Transform(dtoItems);
    }
}