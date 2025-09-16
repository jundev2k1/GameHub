using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Client.Queries.GetGames;

public sealed class GetGamesHandler(
    IGameProviderCacheService gameProviderCache) : IQueryHandler<GetGamesQuery, PaginationResult<GetGamesItemDto>>
{
    public async Task<PaginationResult<GetGamesItemDto>> Handle(GetGamesQuery request, CancellationToken ct = default)
    {
        var searchResult = gameProviderCache.GameList
            .Where(GetFilterCondition(request))
            .ToArray();
        var totalItems = searchResult.Length;
        var items = searchResult
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(MapToListItem)
            .ToArray();
        var result = new PaginationResult<GetGamesItemDto>(
            items: await Task.WhenAll(items) ?? [],
            totalItems: totalItems,
            totalPages: (int)Math.Ceiling((decimal)totalItems / request.PageSize),
            pageIndex: request.PageIndex,
            pageSize: request.PageSize);
        return result;
    }

    private static Func<GameInfoDto, bool> GetFilterCondition(GetGamesQuery request)
    {
        return game =>
            ((request.Platform == null)
                || (game.PlatformId == request.Platform))
            && ((request.Categories == null)
                || game.Categories.Any(c => request.Categories.Contains(c.Id)))
            && ((request.GameTypes == null)
                || game.GameTypes.Any(t => request.GameTypes.Contains(t.Id)))
            && ((request.GameTags == null)
                || game.GameTags.Any(t => request.GameTags.Contains(t.Id)))
            && ((request.Keyword == null)
                || game.Id.ToString().Equals(request.Keyword.Trim(), StringComparison.InvariantCultureIgnoreCase)
                || game.Name.Contains(request.Keyword.Trim(), StringComparison.InvariantCultureIgnoreCase));
    }

    private async Task<GetGamesItemDto> MapToListItem(GameInfoDto game)
    {
        if (game.Thumbnail != null)
        {
            var thumbnailUrl = await gameProviderCache.GetGameThumbnail(game);
            game.Thumbnail.Url = thumbnailUrl;
        }
        var result = new GetGamesItemDto(game);
        return result;
    }
}
