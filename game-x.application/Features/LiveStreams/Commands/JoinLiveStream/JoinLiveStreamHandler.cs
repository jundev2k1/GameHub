using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.LiveStreams.Dtos;
using game_x.share.Settings;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace game_x.application.Features.LiveStreams.Commands.JoinLiveStream;

public sealed class JoinLiveStreamHandler(
    ILiveStreamRepo liveStreamRepo,
    ILiveStreamManagerCacheService liveStreamManager,
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IOptions<SrsSettings> options) : ICommandHandler<JoinLiveStreamCommand, string>
{
    public async Task<string> Handle(JoinLiveStreamCommand request, CancellationToken ct = default)
    {
        var streamSetting = await liveStreamRepo
            .GetByIdAsync(request.Id, ct);
        if (streamSetting.Status != LiveStreamStatus.Live)
            throw new BadRequestException("Live stream is offline.");

        // Check if the live stream has started
        if (streamSetting.StartAt > DateTime.UtcNow)
            throw new BadRequestException("Live stream has not started yet.");

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
                new { Time = targetBlackListItem.BlockTo });

        // Check if the stream is live
        var isInterrupted = !streamInfo.IsLive
            && streamInfo.OfflineAt.HasValue
            && (DateTime.UtcNow - streamInfo.OfflineAt.Value).TotalMinutes < 8;
        if (isInterrupted)
            throw new ForbiddenException(MessageCode.System.Forbidden,"Live streaming is interrupted.", new { isInterrupted = true });

        var viewer = await CreateViewer(streamSetting);
        return viewer.Url;
    }

    private async Task<LiveStreamViewerDto> CreateViewer(LivestreamSchedule schedule)
    {
        var targetUser = await userRepo.GetUserByIdAsync(userAccessor.GetUserId());
        var token = GenerateToken();
        var viewerDto = new LiveStreamViewerDto
        {
            StreamKey = schedule.StreamKey,
            Token = token,
            Url = GenerateUrl(schedule.StreamKey, token),
            ViewerId = targetUser.Id,
            ViewerName = targetUser.Nickname,
            ViewerAvatar = string.Empty,
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
        $"{options.Value.Host}/{streamKey}.flv?token={token}";
}
