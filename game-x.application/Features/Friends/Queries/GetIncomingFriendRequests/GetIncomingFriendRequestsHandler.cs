using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Extensions;
using game_x.application.Features.Friends.Dtos;

namespace game_x.application.Features.Friends.Queries.GetIncomingFriendRequests;

public sealed class GetIncomingFriendRequestsHandler(
    IUserAccessor userAccessor,
    ISocialLinkRepo socialLinkRepo,
    IFileManagerCacheService fileCache,
    ICriteriaBuilder<SocialLink> builder): IQueryHandler<GetIncomingFriendRequestsQuery, PaginationResult<IncomingFriendRequestDto>>
{
    public async Task<PaginationResult<IncomingFriendRequestDto>> Handle(GetIncomingFriendRequestsQuery request, CancellationToken ct)
    {
        var userId = userAccessor.GetUserId();
        var items = await socialLinkRepo.GetIncomingRequestsByCriteriaAsync(
            addresseeUserId: userId,
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword =>
                    link => link.RequesterUser!.Nickname.Contains(keyword)),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);

        var dtoItems = await Task.WhenAll(
            items.Items.Select(async m =>
            {
                var avatar = m.RequesterUser?.Avatar != null ? await fileCache.GetImageUrl(m.RequesterUser.Avatar, ct) : null;
                var linkDto = m.Adapt<SocialLinkDto>();
                return linkDto.Adapt<IncomingFriendRequestDto>() with {RequesterAvatarUrl = avatar?.Url};
            })
        );
        
        return items.Transform(dtoItems);
    }
}