using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetCurrentGameRecommends;

public sealed class GetCurrentGameRecommendsHandler(
    IUserAccessor userAccessor,
    IGameProviderCacheService gameProviderCache) : IQueryHandler<GetCurrentGameRecommendsQuery, GameRecommendListItemDto[]>
{
    public async Task<GameRecommendListItemDto[]> Handle(GetCurrentGameRecommendsQuery request, CancellationToken ct = default)
    {
        var lang = userAccessor.GetLanguage();
        var dateTime = DateTime.UtcNow;
        var data = gameProviderCache.GameRecommendList
            .FirstOrDefault(r => ((r.StartDate ?? DateTime.MinValue) <= dateTime) && ((r.EndDate ?? DateTime.MaxValue) >= dateTime));
        if (data is null) return [];

        var gameList = gameProviderCache.GameList
            .ToDictionary(g => g.LocalId, g => g);
        var tasks = data.Items
            .Where(x => x is {IsActive: true, IsGameActive: true})
            .Select(i => MapToListItem(i, gameList, lang));
        var items = await Task.WhenAll(tasks);
        return items
            .Where(i => i?.IsActive == true)
            .OrderByDescending(i => i!.Priority)
            .ToArray()!;
    }

    private async Task<GameRecommendListItemDto?> MapToListItem(GameRecommendItemDto item, Dictionary<int, GameInfoDto> gameList, string lang)
    {
        if (!gameList.TryGetValue(item.LocalGameId, out var gameInfo)) return null;
        if (gameInfo.Thumbnail != null)
        {
            var thumbnailUrl = await gameProviderCache.GetGameThumbnailAsync(gameInfo);
            gameInfo.Thumbnail.Url = thumbnailUrl;
        }
        if (gameInfo.GameTranslations.Count > 0)
        {
            var targetLang = gameInfo.GameTranslations!.GetValueOrDefault(lang, null);
            if (targetLang != null)
            {
                gameInfo.Name = targetLang.Name;
                gameInfo.Description = targetLang.Description;
                gameInfo.Note = targetLang.Notes;
            }
        }
        var result = new GameRecommendListItemDto(gameInfo, item);
        return result;
    }
}