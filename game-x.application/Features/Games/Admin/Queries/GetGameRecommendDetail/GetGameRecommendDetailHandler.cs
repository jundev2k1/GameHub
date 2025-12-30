using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetGameRecommendDetail;

public sealed class GetGameRecommendDetailHandler(
    IGameProviderCacheService gameProviderCache,
    IGameRecommendRepo gameRecommendRepo) : IQueryHandler<GetGameRecommendDetailQuery, GetGameRecommendDetailDto>
{
    public async Task<GetGameRecommendDetailDto> Handle(GetGameRecommendDetailQuery request, CancellationToken ct = default)
    {
        var targetGame = await gameRecommendRepo.GetAsync(request.Id, ct);
        var dto = targetGame.Adapt<GameRecommendDto>();

        var gameList = gameProviderCache.GameList
            .ToDictionary(g => g.LocalId, g => g);
        var tasks = dto.Items.Select(i => MapToListItem(i, gameList));
        var allItems = await Task.WhenAll(tasks);
        var items = allItems.Where(i => i != null).ToArray()!;
        var result = dto.Adapt<GetGameRecommendDetailDto>();
        result.Items = items!;
        return result;
    }

    private async Task<GameRecommendListItemDto?> MapToListItem(GameRecommendItemDto item, Dictionary<int, GameInfoDto> gameList)
    {
        if (!gameList.TryGetValue(item.LocalGameId, out var gameInfo)) return null;
        if (gameInfo.Thumbnail != null)
        {
            var thumbnailUrl = await gameProviderCache.GetGameThumbnail(gameInfo);
            gameInfo.Thumbnail.Url = thumbnailUrl;
        }
        var result = new GameRecommendListItemDto(gameInfo, item);
        return result;
    }
}
