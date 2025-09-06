using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetGameCategoryDetail;

public sealed class GetGameCategoryDetailHandler(
    IGameProviderCacheService gameProviderCache) : IQueryHandler<GetGameCategoryDetailQuery, GameCategoryDetailDto>
{
    public async Task<GameCategoryDetailDto> Handle(GetGameCategoryDetailQuery request, CancellationToken ct = default)
    {
        var targetGame = gameProviderCache.CategoryList
            .FirstOrDefault(g => g.Id == request.Id)
            ?? throw new NotFoundException(nameof(request.Id), request.Id);

        var relatedGames = gameProviderCache.GameList
            .Where(g => g.Categories != null && g.Categories.Any(c => c.Id == request.Id))
            .Select(g => new CategoryRelatedGameDto
            {
                Id = g.Id,
                Name = g.Name,
                PlatformName = g.PlatformName
            })
            .ToArray();
        var gameResult = targetGame.Adapt<GameCategoryDetailDto>();
        gameResult.RelatedGames = relatedGames;
        return await Task.FromResult(gameResult);
    }
}
