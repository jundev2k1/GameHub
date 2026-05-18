using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;

namespace game_x.application.Features.Games.Common.Queries.GetActivePlatforms;

public sealed class GetActivePlatformsHandler(
    IUserAccessor userAccessor,
    IGameProviderCacheService gameProviderCache) : IQueryHandler<GetActivePlatformsQuery, GetActivePlatformsResult[]>
{
    public async Task<GetActivePlatformsResult[]> Handle(GetActivePlatformsQuery request, CancellationToken ct = default)
    {
        var lang = userAccessor.GetLanguage();
        var result = gameProviderCache.PlatformList
            .Where(cate => cate.IsActive)
            .OrderByDescending(cate => cate.Priority)
            .Select(cate =>
            {
                cate.PlatformTranslations.TryGetValue(lang, out var translation);
                return new GetActivePlatformsResult(
                    cate.Id,
                    translation?.Name ?? cate.Name,
                    translation?.Description ?? cate.Description);
            })
            .ToArray();

        return await Task.FromResult(result);
    }
}
