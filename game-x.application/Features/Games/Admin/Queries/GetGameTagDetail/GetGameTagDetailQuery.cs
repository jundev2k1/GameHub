using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetGameTagDetail;

public record GetGameTagDetailQuery(Guid Id) : IQuery<GameTagDetailDto>;
