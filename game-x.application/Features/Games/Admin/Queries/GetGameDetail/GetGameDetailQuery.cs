using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetGameDetail;

public record GetGameDetailQuery(Guid Id) : IQuery<GameDetailDto>;
