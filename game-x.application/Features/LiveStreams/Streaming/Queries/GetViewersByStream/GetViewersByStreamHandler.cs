using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.LiveStreams.Streaming.Dtos;

namespace game_x.application.Features.LiveStreams.Streaming.Queries.GetViewersByStream;

public sealed class GetViewersByStreamHandler(
    IUserAccessor userAccessor,
    ILiveStreamRepo liveStreamRepo,
    ILiveStreamManagerCacheService liveStreamManager) : IQueryHandler<GetViewersByStreamQuery, LiveStreamViewerInfoDto[]>
{
    public async Task<LiveStreamViewerInfoDto[]> Handle(GetViewersByStreamQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var targetSchedule = await liveStreamRepo.GetByStreamKeyAsync(request.StreamKey, ct);
        var isOwner = targetSchedule.AssignedId == userId;
        if (!isOwner)
            throw new BadRequestException("You are not the owner of this stream.");

        var viewerInfos = liveStreamManager.GetAllViewersByStreamKey(request.StreamKey);
        if (viewerInfos.Count == 0) return [];

        var result = viewerInfos
            .Select(kvp => GetViewerInfo(request.StreamKey, kvp))
            .Where(v => v is not null)
            .ToArray();
        return result!;
    }

    private LiveStreamViewerInfoDto? GetViewerInfo(string streamKey, KeyValuePair<string, string[]> kvp)
    {
        var ViewerAccordingDevices = kvp.Value
            .Select(token => liveStreamManager.GetViewerInfo(streamKey, token))
            .Where(v => v is not null)
            .ToArray();
        if (ViewerAccordingDevices.Length == 0) return null;

        var firstViewer = ViewerAccordingDevices[0]!;
        var devices = ViewerAccordingDevices
            .Select(v => new ViewerDeviceInfoDto
            {
                DeviceName = v!.DeviceInfo,
                IsWatching = v.IsWatching,
                JoinAt = v.JoinAt,
                OutAt = v.OutAt
            })
            .ToArray();
        return new LiveStreamViewerInfoDto
        {
            ViewerId = kvp.Key,
            ViewerName = firstViewer.ViewerName,
            ViewerAvatar = firstViewer.ViewerAvatar,
            DeviceInfos = devices
        };
    }
}
