using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetCurrentGameRecommends;

public sealed class GetCurrentGameRecommendsHandler(
    IGameProviderCacheService gameProviderCache) : IQueryHandler<GetCurrentGameRecommendsQuery, GameRecommendListItemDto[]>
{
    public async Task<GameRecommendListItemDto[]> Handle(GetCurrentGameRecommendsQuery request, CancellationToken ct = default)
    {
        var dateTime = DateTime.UtcNow;
        var data = gameProviderCache.GameRecommendList
            .FirstOrDefault(r => ((r.StartDate ?? DateTime.MinValue) <= dateTime) && ((r.EndDate ?? DateTime.MaxValue) >= dateTime));
        if (data is null) return [];

        var gameList = gameProviderCache.GameList
            .ToDictionary(g => g.LocalId, g => g);
        var tasks = data.Items.Select(i => MapToListItem(i, gameList));
        var items = await Task.WhenAll(tasks);
        return items.Where(i => i != null).ToArray()!;
    }

    private async Task<GameRecommendListItemDto?> MapToListItem(GameRecommendItemDto item, Dictionary<int, GameInfoDto> gameList)
    {
        if (!gameList.TryGetValue(item.LocalGameId, out var gameInfo)) return null;
        if (gameInfo.Thumbnail != null)
        {
            var thumbnailUrl = await gameProviderCache.GetGameThumbnail(gameInfo);
            gameInfo.Thumbnail.Url = thumbnailUrl;
        }
        var result = new GameRecommendListItemDto(gameInfo, item);
        return result;
    }
}
