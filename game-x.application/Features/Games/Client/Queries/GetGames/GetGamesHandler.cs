using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Client.Queries.GetGames;

public sealed class GetGamesHandler(
    IUserAccessor userAccessor,
    IGameProviderCacheService gameProviderCache) : IQueryHandler<GetGamesQuery, PaginationResult<GameItemDto>>
{
    public async Task<PaginationResult<GameItemDto>> Handle(GetGamesQuery request, CancellationToken ct = default)
    {
        var language = userAccessor.GetLanguage();
        var searchResult = gameProviderCache.GameList
            .OrderByDescending(g => g.Priority)
            .Where(GetFilterCondition(request, language))
            .ToArray();
        var totalItems = searchResult.Length;
        var items = searchResult
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(i => MapToListItem(i, language))
            .ToArray();
        var result = new PaginationResult<GameItemDto>(
            items: await Task.WhenAll(items),
            totalItems: totalItems,
            totalPages: (int)Math.Ceiling((decimal)totalItems / request.PageSize),
            pageIndex: request.PageIndex,
            pageSize: request.PageSize);
        return result;
    }

    private static Func<GameInfoDto, bool> GetFilterCondition(GetGamesQuery request, string language)
    {
        var searchKey = request.Keyword?.Trim();
        return game => game.IsActive
            && ((request.Platform == null)
                || (game.PlatformId == request.Platform))
            && ((request.Categories == null)
                || game.Categories.Any(c => request.Categories.Contains(c.Id)))
            && ((request.GameTypes == null)
                || game.GameTypes.Any(t => request.GameTypes.Contains(t.Id)))
            && ((request.GameTags == null)
                || game.GameTags.Any(t => request.GameTags.Contains(t.Id)))
            && ((searchKey == null)
                || game.Id.ToString().Equals(searchKey, StringComparison.InvariantCultureIgnoreCase)
                || ((game.GameTranslations.Count == 0) || !game.GameTranslations.TryGetValue(language, out GameTranslationInfo? value)
                    ? game.Name.Contains(searchKey, StringComparison.InvariantCultureIgnoreCase)
                    : value.Name.Contains(searchKey, StringComparison.InvariantCultureIgnoreCase)));
    }

    private async Task<GameItemDto> MapToListItem(GameInfoDto game, string lang)
    {
        // Map thumbnail
        if (game.Thumbnail != null)
        {
            var thumbnailUrl = await gameProviderCache.GetGameThumbnailAsync(game);
            game.Thumbnail.Url = thumbnailUrl;
        }

        // Map game platform translation
        var targetPlatform = gameProviderCache.PlatformList.FirstOrDefault(c => c.LocalId == game.LocalId);
        if (targetPlatform != null && targetPlatform.PlatformTranslations.TryGetValue(lang, out var platformTranslation))
        {
            game.PlatformName = platformTranslation.Name;
        }

        // Map game translation
        if (game.GameTranslations.Count > 0)
        {
            var targetLang = game.GameTranslations!.GetValueOrDefault(lang, null);
            if (targetLang != null)
            {
                game.Name = targetLang.Name;
                game.Description = targetLang.Description;
                game.Note = targetLang.Note;
            }
        }

        // Map game category translations
        if (game.Categories.Length > 0)
        {
            foreach (var category in game.Categories)
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
        if (game.GameTypes.Length > 0)
        {
            foreach (var type in game.GameTypes)
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
        if (game.GameTags.Length > 0)
        {
            foreach (var tag in game.GameTags)
            {
                var targetTag = gameProviderCache.GameTagList.FirstOrDefault(c => c.LocalId == tag.LocalId);
                if (targetTag != null && targetTag.TagTranslations.TryGetValue(lang, out var translation))
                {
                    tag.Name = translation.Name;
                    tag.Description = translation.Description;
                }
            }
        }

        // Map game media items
        if (game.GameMediaItems.Length > 0)
        {
            var mediaItems = await gameProviderCache.GetGameMediasAsync(game);
            game.GameMediaItems = mediaItems;
        }

        var result = new GameItemDto(game);
        return result;
    }
}
