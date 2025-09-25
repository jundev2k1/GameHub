using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Extensions;
using game_x.application.Features.Friends.Dtos;

namespace game_x.application.Features.Friends.Queries.GetBlockedFriends;

public sealed class GetBlockedFriendsHandler(
    IUserAccessor userAccessor,
    ISocialLinkRepo socialLinkRepo,
    IFileManagerCacheService fileCache,
    ICriteriaBuilder<SocialLink> builder): IQueryHandler<GetBlockedFriendsQuery, PaginationResult<BlockedFriendDto>>
{
    public async Task<PaginationResult<BlockedFriendDto>> Handle(GetBlockedFriendsQuery request, CancellationToken ct)
    {
        var userId = userAccessor.GetUserId();
        var items = await socialLinkRepo.GetBlockedFriendsAsync(
            userId,
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword =>
                    link => link.BlockedUser!.Nickname.Contains(keyword)),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);

        var dtoItems = await Task.WhenAll(
            items.Items.Select(async m =>
            {
                var avatarUrl = m.BlockedUser?.Avatar != null ? await fileCache.GetFileUrl(m.BlockedUser.Avatar, ct) : null;
                var linkDto = m.Adapt<SocialLinkDto>();
                return linkDto.Adapt<BlockedFriendDto>() with {BlockedAvatarUrl = avatarUrl};
            })
        );
        
        return items.Transform(dtoItems);
    }
}