using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetGameTypeDetail;

public record GetGameTypeDetailQuery(Guid Id) : IQuery<GameTypeDetailDto>;
