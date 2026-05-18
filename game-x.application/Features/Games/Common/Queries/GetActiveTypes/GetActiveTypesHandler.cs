using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;

namespace game_x.application.Features.Games.Common.Queries.GetActiveTypes;

public sealed class GetActiveTypesHandler(
    IUserAccessor userAccessor,
    IGameProviderCacheService gameProviderCache) : IQueryHandler<GetActiveTypesQuery, GetActiveTypesResult[]>
{
    public async Task<GetActiveTypesResult[]> Handle(GetActiveTypesQuery request, CancellationToken ct = default)
    {
        var lang = userAccessor.GetLanguage();
        var result = gameProviderCache.GameTypeList
            .Where(cate => cate.IsActive)
            .OrderByDescending(cate => cate.Priority)
            .Select(cate =>
            {
                cate.TypeTranslations.TryGetValue(lang, out var translation);
                return new GetActiveTypesResult(
                    cate.Id,
                    translation?.Name ?? cate.Name,
                    translation?.Description ?? cate.Description);
            })
            .ToArray();

        return await Task.FromResult(result);
    }
}
