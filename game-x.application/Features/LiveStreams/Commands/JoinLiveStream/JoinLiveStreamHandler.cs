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

        if (streamSetting.StartAt > DateTime.UtcNow)
            throw new BadRequestException("Live stream has not started yet.");

        var streamInfo = liveStreamManager.GetLiveStreamStatus(streamSetting.StreamKey);
        if (streamInfo is null || !streamInfo.IsLive)
            throw new NotFoundException("Live stream is offline.");

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
        $"{options.Value.StreamServer}/{streamKey}.flv?token={token}";
}
