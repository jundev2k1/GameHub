using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.LiveStreams.Streaming.Dtos;
using game_x.share.Settings;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace game_x.application.Features.LiveStreams.Streaming.Commands.JoinLiveStream;

public sealed class JoinLiveStreamHandler(
    ILiveStreamRepo liveStreamRepo,
    ILiveStreamManagerCacheService liveStreamManager,
    IFileManagerCacheService fileManagerCache,
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IOptions<SrsSettings> options) : ICommandHandler<JoinLiveStreamCommand, JoinLiveStreamResult>
{
    public async Task<JoinLiveStreamResult> Handle(JoinLiveStreamCommand request, CancellationToken ct = default)
    {
        var streamSetting = await liveStreamRepo
            .GetByIdAsync(request.Id, ct);
        if (streamSetting.Status != LiveStreamStatus.Live)
            throw new ForbiddenException("Live stream is offline.");

        if (streamSetting.Status == LiveStreamStatus.Cancelled)
            throw new ForbiddenException(
                MessageCode.System.Forbidden,
                "Live stream has been canceled.",
                new { streamSetting.CancellationReason });

        // Check if the live stream has started
        if (streamSetting.StartTime > DateTime.UtcNow)
            throw new ForbiddenException(
                MessageCode.System.Forbidden,
                "Live stream has not started yet.",
                new { streamSetting.StartTime });

        // Get the live stream status from cache
        var streamInfo = liveStreamManager.GetLiveStreamStatus(streamSetting.StreamKey)
            ?? throw new NotFoundException("Live stream is not found.");

        // Check if the user is blocked from viewing the stream
        var targetBlackListItem = streamInfo.BlackList
            .FirstOrDefault(i => i.UserId == userAccessor.GetUserId()
                && i.Action == BlackListAction.View
                && i.BlockTo < DateTime.UtcNow);
        if (targetBlackListItem != null)
            throw new ForbiddenException(
                MessageCode.System.Forbidden,
                "You are blocked from viewing this live stream.",
                new { targetBlackListItem.Action, targetBlackListItem.BlockTo, targetBlackListItem.Reason });

        // Check if the stream is live
        var isInterrupted = !streamInfo.IsLive
            && streamInfo.OfflineAt.HasValue
            && (DateTime.UtcNow - streamInfo.OfflineAt.Value).TotalMinutes < 8;
        if (isInterrupted)
            throw new ForbiddenException(
                MessageCode.System.Forbidden,
                "Live streaming is interrupted.",
                new { isInterrupted = true });

        var viewer = await CreateViewer(streamSetting);
        return new JoinLiveStreamResult(
            streamInfo.Title,
            streamInfo.Description,
            streamInfo.Thumbnail,
            streamInfo.StreamKey,
            streamInfo.LiveAt ?? DateTime.UtcNow,
            streamInfo.AssignedTo?.Id ?? string.Empty,
            streamInfo.AssignedTo?.Nickname ?? string.Empty,
            streamInfo.AssignedTo?.Avatar ?? string.Empty,
            liveStreamManager.GetViewerCount(streamInfo.StreamKey),
            viewer.Url);
    }

    private async Task<LiveStreamViewerDto> CreateViewer(LivestreamSchedule schedule)
    {
        var targetUser = await userRepo.GetUserByIdAsync(userAccessor.GetUserId());
        var token = GenerateToken();
        var avatarInfo = targetUser.Avatar != null
            ? await fileManagerCache.GetImageUrl(targetUser.Avatar)
            : null;
        var viewerDto = new LiveStreamViewerDto
        {
            StreamKey = schedule.StreamKey,
            Token = token,
            Url = GenerateUrl(schedule.StreamKey, token),
            ViewerId = targetUser.Id,
            ViewerName = targetUser.Nickname,
            ViewerAvatar = avatarInfo?.Url,
            DeviceInfo = userAccessor.GetDeviceInfo()
        };
        liveStreamManager.InitViewerLiveStream(viewerDto);

        return viewerDto;
    }

    private static string GenerateToken() =>
        Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64))
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');

    private string GenerateUrl(string streamKey, string token) =>
        $"{options.Value.ClientUrl}/{streamKey}.flv?token={token}";
}
