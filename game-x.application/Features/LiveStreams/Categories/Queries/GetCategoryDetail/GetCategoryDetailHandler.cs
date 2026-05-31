using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.LiveStreams.Categories.Dtos;

namespace game_x.application.Features.LiveStreams.Categories.Queries.GetCategoryDetail;

public sealed class GetCategoryDetailHandler(
    ILiveStreamCategoryRepo liveStreamCategoryRepo) : IQueryHandler<GetCategoryDetailQuery, LiveStreamCategoryDto>
{
    public async Task<LiveStreamCategoryDto> Handle(GetCategoryDetailQuery request, CancellationToken ct = default)
    {
        var result = await liveStreamCategoryRepo.GetByIdAsync(request.Id, ct);
        return result.Adapt<LiveStreamCategoryDto>();
    }
}
