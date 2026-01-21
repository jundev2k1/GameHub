using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Extensions;
using game_x.application.Features.Friends.Dtos;


namespace game_x.application.Features.Friends.Queries.FriendSearch;

public class FriendSearchHandler(
    IUserAccessor userAccessor,
    ISocialLinkRepo socialLinkRepo,
    IFileManagerCacheService fileCache,
    ICriteriaBuilder<FriendSearchDto> builder): IQueryHandler<FriendSearchQuery, PaginationResult<FriendSearchResultDto>>
{
    public async Task<PaginationResult<FriendSearchResultDto>> Handle(FriendSearchQuery request, CancellationToken ct)
    {
        string userId = userAccessor.GetUserId();
        var items = await socialLinkRepo.SearchFriendshipsAsync(
            userId,
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword => user => 
                    user.Nickname.ToLower().Contains(keyword.ToLower()) || 
                    user.Email.ToLower().Contains(keyword.ToLower())),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);
        
        var dtoItems = await Task.WhenAll(
            items.Items.Select(async m =>
            {
                var avatarUrl = m.Avatar != null ? await fileCache.GetFileUrl(m.Avatar, ct) : null;
                return m.Adapt<FriendSearchResultDto>() with {AvatarUrl = avatarUrl};
            })
        );
        
        return items.Transform(dtoItems);
    }
}