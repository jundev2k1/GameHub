using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.LiveStreams.Dtos;

namespace game_x.application.Features.LiveStreams.Queries.GetScheduleDetail;

public sealed class GetScheduleDetailHandler(
    ILiveStreamRepo liveStreamRepo) : IQueryHandler<GetScheduleDetailQuery, LiveStreamScheduleDto>
{
    public async Task<LiveStreamScheduleDto> Handle(GetScheduleDetailQuery request, CancellationToken ct = default)
    {
        var result = await liveStreamRepo.GetByIdAsync(request.Id, ct);
        return result.Adapt<LiveStreamScheduleDto>();
    }
}
