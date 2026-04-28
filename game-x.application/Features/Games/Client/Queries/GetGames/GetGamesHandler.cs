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
        if (game.Thumbnail != null)
        {
            var thumbnailUrl = await gameProviderCache.GetGameThumbnail(game);
            game.Thumbnail.Url = thumbnailUrl;
        }

        if (game.GameTranslations.Count > 0)
        {
            var targetLang = game.GameTranslations!.GetValueOrDefault(lang, null);
            if (targetLang != null)
            {
                game.Name = targetLang.Name;
                game.Description = targetLang.Description;
                game.Note = targetLang.Notes;
            }
        }
        var result = new GameItemDto(game);
        return result;
    }
}
