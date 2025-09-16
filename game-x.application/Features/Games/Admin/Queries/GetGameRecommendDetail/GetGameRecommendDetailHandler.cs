using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetGameRecommendDetail;

public sealed class GetGameRecommendDetailHandler(
    IGameProviderCacheService gameProviderCache) : IQueryHandler<GetGameRecommendDetailQuery, GameRecommendDto>
{
    public async Task<GameRecommendDto> Handle(GetGameRecommendDetailQuery request, CancellationToken ct = default)
    {
        var targetGame = gameProviderCache.CategoryList
            .FirstOrDefault(g => g.Id == request.Id)
            ?? throw new NotFoundException(nameof(request.Id), request.Id);

        var result = targetGame.Adapt<GameRecommendDto>();
        return await Task.FromResult(result);
    }
}
