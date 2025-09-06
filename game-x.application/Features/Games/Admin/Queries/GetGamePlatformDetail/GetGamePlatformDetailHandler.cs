using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetGamePlatformDetail;

public sealed class GetGamePlatformDetailHandler(
    IGameProviderCacheService gameProviderCache) : IQueryHandler<GetGamePlatformDetailQuery, GamePlatformDetailDto>
{
    public async Task<GamePlatformDetailDto> Handle(GetGamePlatformDetailQuery request, CancellationToken ct = default)
    {
        var targetGame = gameProviderCache.PlatformList
            .FirstOrDefault(g => g.Id == request.Id)
            ?? throw new NotFoundException(nameof(request.Id), request.Id);

        var relatedGames = gameProviderCache.GameList
            .Where(g => g.PlatformId == request.Id)
            .Select(g => new PlatformRelatedGameDto
            {
                Id = g.Id,
                Name = g.Name
            })
            .ToArray();
        var gameResult = targetGame.Adapt<GamePlatformDetailDto>();
        gameResult.RelatedGames = relatedGames;
        return await Task.FromResult(gameResult);
    }
}
