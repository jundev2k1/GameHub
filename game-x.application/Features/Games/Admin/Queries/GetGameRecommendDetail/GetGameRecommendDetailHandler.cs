using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetGameRecommendDetail;

public sealed class GetGameRecommendDetailHandler(
    IGameRecommendRepo gameRecommendRepo) : IQueryHandler<GetGameRecommendDetailQuery, GameRecommendDto>
{
    public async Task<GameRecommendDto> Handle(GetGameRecommendDetailQuery request, CancellationToken ct = default)
    {
        var targetGame = await gameRecommendRepo.GetAsync(request.Id, ct);
        var result = targetGame.Adapt<GameRecommendDto>();
        return result;
    }
}
