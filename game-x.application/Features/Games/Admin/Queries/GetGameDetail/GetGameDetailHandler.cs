using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetGameDetail;

public sealed class GetGameDetailHandler(
    IGameProviderCacheService gameProviderCache,
    IFileManagerCacheService fileManagerCache) : IQueryHandler<GetGameDetailQuery, GameDetailDto>
{
    public async Task<GameDetailDto> Handle(GetGameDetailQuery request, CancellationToken ct = default)
    {
        var targetGame = gameProviderCache.GameList
            .FirstOrDefault(g => g.Id == request.Id)
            ?? throw new NotFoundException(nameof(request.Id), request.Id);

        var result = targetGame.Adapt<GameDetailDto>();
        if (result.Thumbnail != null)
        {
            var thumbnail = await fileManagerCache.GetFileUrl(result.Thumbnail.LocalId!, ct);
            result.Thumbnail.Url = thumbnail?.Url;
        }

        return await Task.FromResult(result);
    }
}
