using game_x.application.Features.LiveStreams.Dtos;

namespace game_x.application.Features.LiveStreams.Queries.GetCategoryDetail;

public record GetCategoryDetailQuery(Guid Id) : IQuery<LiveStreamCategoryDto>;
