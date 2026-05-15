using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;

namespace game_x.application.Features.Games.Common.Queries.GetActiveCategories;

public sealed class GetActiveCategoriesHandler(
    IUserAccessor userAccessor,
    IGameProviderCacheService gameProviderCache) : IQueryHandler<GetActiveCategoriesQuery, GetActiveCategoriesResult[]>
{
    public async Task<GetActiveCategoriesResult[]> Handle(GetActiveCategoriesQuery request, CancellationToken ct = default)
    {
        var lang = userAccessor.GetLanguage();
        var result = gameProviderCache.CategoryList
            .Where(cate => cate.IsActive)
            .OrderByDescending(cate => cate.Priority)
            .Select(cate =>
            {
                cate.CategoryTranslations.TryGetValue(lang, out var translation);
                return new GetActiveCategoriesResult(
                    cate.Id,
                    translation?.Name ?? cate.Name,
                    translation?.Description ?? cate.Description);
            })
            .ToArray();

        return await Task.FromResult(result);
    }
}
