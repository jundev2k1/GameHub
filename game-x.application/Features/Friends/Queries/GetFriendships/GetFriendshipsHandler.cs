using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Extensions;
using game_x.application.Features.Friends.Dtos;

namespace game_x.application.Features.Friends.Queries.GetFriendships;

public class GetFriendshipsHandler(
    IUserAccessor userAccessor, 
    ISocialLinkRepo socialLinkRepo,
    IFileManagerCacheService fileCache,
    ICriteriaBuilder<FriendDto> builder): IQueryHandler<GetFriendshipsQuery, PaginationResult<ListedFriendDto>>
{
    public async Task<PaginationResult<ListedFriendDto>> Handle(GetFriendshipsQuery request, CancellationToken ct)
    {
        string userId = userAccessor.GetUserId();
        
        var items = await socialLinkRepo.GetFriendshipsAsync(
            userId,
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword =>
                    user =>
                        (user.Nickname.Contains(keyword))),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);

        var dtoItems = await Task.WhenAll(
            items.Items.Select(async m =>
            {
                var avatarUrl = m.Avatar != null ? await fileCache.GetFileUrl(m.Avatar, ct) : null;
                return m.Adapt<ListedFriendDto>() with {AvatarUrl = avatarUrl};
            })
        );

        return items.Transform(dtoItems);
    }
}