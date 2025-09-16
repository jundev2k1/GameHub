using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetGameRecommendDetail;

public record GetGameRecommendDetailQuery(Guid Id) : IQuery<GameRecommendDto>;
