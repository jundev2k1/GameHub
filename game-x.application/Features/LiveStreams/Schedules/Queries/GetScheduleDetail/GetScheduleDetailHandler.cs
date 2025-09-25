using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.application.Features.LiveStreams.Schedules.Queries.GetScheduleDetail;

public sealed class GetScheduleDetailHandler(
    ILiveStreamRepo liveStreamRepo,
    IOptions<SrsSettings> options,
    IFileManagerCacheService fileManagerCache) : IQueryHandler<GetScheduleDetailQuery, GetScheduleDetailResult>
{
    public async Task<GetScheduleDetailResult> Handle(GetScheduleDetailQuery request, CancellationToken ct = default)
    {
        var targetStream = await liveStreamRepo.GetDetailByIdAsync(request.Id, ct);
        var result = targetStream.Adapt<GetScheduleDetailResult>();
        result.StreamUrl = options.Value.StreamServer;
        result.StreamKey = $"{result.StreamKey}?token={targetStream.Token}";

        // Load avatar for assigned talent
        if (targetStream.AssignedTo != null && targetStream.AssignedTo.Avatar != null)
        {
            var avatarInfo = await fileManagerCache.GetFileUrl(targetStream.AssignedTo.Avatar!, ct);
            result.AssignedTo!.Avatar = avatarInfo?.Url;
        }

        // Load thumbnail
        if (targetStream.ThumbnailId.HasValue)
        {
            var thumbnail = await fileManagerCache.GetFileUrl(targetStream.ThumbnailId.Value, ct);
            result.ThumbnailUrl = thumbnail?.Url;
        }

        return result;
    }
}
