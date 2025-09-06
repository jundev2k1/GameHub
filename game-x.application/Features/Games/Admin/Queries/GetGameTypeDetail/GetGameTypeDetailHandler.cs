using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetGameTypeDetail;

public sealed class GetGameTypeDetailHandler(
    IGameProviderCacheService gameProviderCache) : IQueryHandler<GetGameTypeDetailQuery, GameTypeDetailDto>
{
    public async Task<GameTypeDetailDto> Handle(GetGameTypeDetailQuery request, CancellationToken ct = default)
    {
        var targetGame = gameProviderCache.GameTypeList
            .FirstOrDefault(g => g.Id == request.Id)
            ?? throw new NotFoundException(nameof(request.Id), request.Id);

        var relatedGames = gameProviderCache.GameList
            .Where(g => g.GameTypes != null && g.GameTypes.Any(c => c.Id == request.Id))
            .Select(g => new GameTypeRelatedGameDto
            {
                Id = g.Id,
                Name = g.Name,
                PlatformName = g.PlatformName
            })
            .ToArray();
        var gameResult = targetGame.Adapt<GameTypeDetailDto>();
        gameResult.RelatedGames = relatedGames;
        return await Task.FromResult(gameResult);
    }
}
