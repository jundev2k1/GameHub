using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetGameCategoryDetail;

public record GetGameCategoryDetailQuery(Guid Id) : IQuery<GameCategoryDetailDto>;
