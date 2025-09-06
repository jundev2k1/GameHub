using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetGamePlatformDetail;

public record GetGamePlatformDetailQuery(Guid Id) : IQuery<GamePlatformDetailDto>;
