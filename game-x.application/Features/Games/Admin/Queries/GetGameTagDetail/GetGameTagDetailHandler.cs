using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetGameTagDetail;

public sealed class GetGameTagDetailHandler(
    IGameProviderCacheService gameProviderCache) : IQueryHandler<GetGameTagDetailQuery, GameTagDetailDto>
{
    public async Task<GameTagDetailDto> Handle(GetGameTagDetailQuery request, CancellationToken ct = default)
    {
        var targetGame = gameProviderCache.GameTagList
            .FirstOrDefault(g => g.Id == request.Id)
            ?? throw new NotFoundException(nameof(request.Id), request.Id);

        var relatedGames = gameProviderCache.GameList
            .Where(g => g.GameTags != null && g.GameTags.Any(c => c.Id == request.Id))
            .Select(g => new GameTagRelatedGameDto
            {
                Id = g.Id,
                Name = g.Name,
                PlatformName = g.PlatformName,
            })
            .ToArray();
        var gameResult = targetGame.Adapt<GameTagDetailDto>();
        gameResult.RelatedGames = relatedGames;
        return await Task.FromResult(gameResult);
    }
}
