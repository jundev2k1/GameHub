using game_x.application.Contract.Persistence.Repo;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.application.Features.LiveStreams.Queries.GetScheduleDetail;

public sealed class GetScheduleDetailHandler(
    ILiveStreamRepo liveStreamRepo,
    IOptions<SrsSettings> options) : IQueryHandler<GetScheduleDetailQuery, GetScheduleDetailResult>
{
    public async Task<GetScheduleDetailResult> Handle(GetScheduleDetailQuery request, CancellationToken ct = default)
    {
        var targetStream = await liveStreamRepo.GetByIdAsync(request.Id, ct);
        var result = targetStream.Adapt<GetScheduleDetailResult>();
        result.StreamUrl = $"{options.Value.StreamServer}?token={targetStream.Token}";
        return result.Adapt<GetScheduleDetailResult>();
    }
}
