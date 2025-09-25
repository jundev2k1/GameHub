using game_x.application.Features.LiveStreams.Categories.Dtos;

namespace game_x.application.Features.LiveStreams.Categories.Queries.GetCategoryDetail;

public record GetCategoryDetailQuery(Guid Id) : IQuery<LiveStreamCategoryDto>;
