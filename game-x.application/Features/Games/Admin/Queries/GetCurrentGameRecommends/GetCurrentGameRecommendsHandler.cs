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
            .Where(x => x is { IsActive: true, IsGameActive: true })
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

        // Map thumbnail
        if (gameInfo.Thumbnail != null)
        {
            var thumbnailUrl = await gameProviderCache.GetGameThumbnailAsync(gameInfo);
            gameInfo.Thumbnail.Url = thumbnailUrl;
        }

        // Map game platform translation
        var targetPlatform = gameProviderCache.PlatformList.FirstOrDefault(c => c.LocalId == gameInfo.LocalId);
        if (targetPlatform != null && targetPlatform.PlatformTranslations.TryGetValue(lang, out var platformTranslation))
        {
            gameInfo.PlatformName = platformTranslation.Name;
        }

        // Map game translation
        if (gameInfo.GameTranslations.Count > 0)
        {
            var targetLang = gameInfo.GameTranslations!.GetValueOrDefault(lang, null);
            if (targetLang != null)
            {
                gameInfo.Name = targetLang.Name;
                gameInfo.Description = targetLang.Description;
                gameInfo.Note = targetLang.Note;
            }
        }

        // Map game category translations
        if (gameInfo.Categories.Length > 0)
        {
            foreach (var category in gameInfo.Categories)
            {
                var targetCategory = gameProviderCache.CategoryList.FirstOrDefault(c => c.LocalId == category.LocalId);
                if (targetCategory != null && targetCategory.CategoryTranslations.TryGetValue(lang, out var translation))
                {
                    category.Name = translation.Name;
                    category.Description = translation.Description;
                }
            }
        }

        // Map game type translations
        if (gameInfo.GameTypes.Length > 0)
        {
            foreach (var type in gameInfo.GameTypes)
            {
                var targetType = gameProviderCache.GameTypeList.FirstOrDefault(c => c.LocalId == type.LocalId);
                if (targetType != null && targetType.TypeTranslations.TryGetValue(lang, out var translation))
                {
                    type.Name = translation.Name;
                    type.Description = translation.Description;
                }
            }
        }

        // Map game tag translations
        if (gameInfo.GameTags.Length > 0)
        {
            foreach (var tag in gameInfo.GameTags)
            {
                var targetTag = gameProviderCache.GameTagList.FirstOrDefault(c => c.LocalId == tag.LocalId);
                if (targetTag != null && targetTag.TagTranslations.TryGetValue(lang, out var translation))
                {
                    tag.Name = translation.Name;
                    tag.Description = translation.Description;
                }
            }
        }

        var result = new GameRecommendListItemDto(gameInfo, item);
        return result;
    }
}