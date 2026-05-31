using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetCurrentGameRecommends;

public record GetCurrentGameRecommendsQuery(RecommendationType Type) : IQuery<GameRecommendListItemDto[]>;
