using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.Srs;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.LiveStream;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.LiveStreams.Dtos;
using game_x.application.Features.LiveStreams.Enum;

namespace game_x.application.Features.LiveStreams.Commands.PerformAction;

public sealed class PerformActionHandler(
    IUserAccessor userAccessor,
    ILiveStreamRepo liveStreamRepo,
    ILiveStreamManagerCacheService liveStreamManager,
    ISrsService srsService,
    ILiveStreamHubService liveStreamHub) : ICommandHandler<PerformActionCommand>
{
    public async Task<Unit> Handle(PerformActionCommand request, CancellationToken ct = default)
    {
        // Check if the stream exists and is live
        var targetSchedule = await liveStreamRepo.GetByStreamKeyAsync(request.StreamKey!, ct);
        if (targetSchedule.AssignedId != userAccessor.GetUserId())
            throw new BadRequestException("You are not the owner of this stream.");

        if (targetSchedule.AssignedId == request.ViewerId)
            throw new BadRequestException("You cannot perform action on yourself.");

        if (targetSchedule.Status != LiveStreamStatus.Live)
            throw new BadRequestException("The stream is not live.");

        var targetStream = liveStreamManager.GetLiveStreamStatus(targetSchedule.StreamKey)
            ?? throw new BadRequestException("The stream is not live.");

        // Check if the viewer is watching the stream
        var targetViewerInfo = liveStreamManager.GetAllViewersByStreamKey(targetSchedule.StreamKey)
            .FirstOrDefault(kvp => kvp.Key == request.ViewerId);
        if (targetViewerInfo.Value.Length == 0)
            throw new BadRequestException("The viewer is not watching this stream.");

        // Find the viewer in the list
        LiveStreamViewerDto? targetViewer = null;
        var index = 0;
        while (targetViewer != null)
        {
            targetViewer = liveStreamManager.GetViewerInfo(targetSchedule.StreamKey, targetViewerInfo.Value[index]);
            index++;
        }
        if (targetViewer is null)
            throw new BadRequestException("The viewer is not watching this stream.");

        // Perform action
        switch (request.Action)
        {
            case PerformActionEnum.Kick:
                await KickViewer(targetSchedule.StreamKey, targetViewer, request.BlockTime!.Value, request.Reason!.Value);
                break;

            case PerformActionEnum.Unkick:
                await UnkickViewer(targetSchedule.StreamKey, request.ViewerId!);
                break;

            case PerformActionEnum.Mute:
                await MuteViewer(targetSchedule.StreamKey, targetViewer, request.BlockTime!.Value, request.Reason!.Value);
                break;

            case PerformActionEnum.Unmute:
                await UnmuteViewer(targetSchedule.StreamKey, request.ViewerId!);
                break;

            case PerformActionEnum.BlockDonation:
                await BlockDonation(targetSchedule.StreamKey, targetViewer, request.BlockTime!.Value, request.Reason!.Value);
                break;

            case PerformActionEnum.UnblockDonation:
                await UnblockDonation(targetSchedule.StreamKey, request.ViewerId!);
                break;

            default:
                throw new BadRequestException($"Action ({request.Action}) is invalid.");
        }
        return Unit.Value;
    }

    private async Task KickViewer(string streamKey, LiveStreamViewerDto viewer, int minutes, BlockReasonEnum reason)
    {
        // Add the viewer to the blacklist for block rejoining
        var blackListItem = new BlackListItemDto
        {
            UserId = viewer.ViewerId,
            Username = viewer.ViewerName,
            Action = BlackListAction.View,
            BlockTo = DateTime.UtcNow.AddMinutes(minutes),
            Reason = reason,
        };
        liveStreamManager.AddBlackList(streamKey, blackListItem);

        // Kick the viewer from SRS
        await srsService.KickClientAsync(viewer.ClientId);

        // Notify the viewer via SignalR
        await liveStreamHub.PerformActionMember(
            streamKey,
            blackListItem.UserId,
            new LiveStreamBanInfo { Action = PerformActionEnum.Kick, BanUntil = blackListItem.BlockTo, Reason = reason });
    }

    private async Task UnkickViewer(string streamKey, string viewerId)
    {
        liveStreamManager.RemoveBlackList(streamKey, viewerId, BlackListAction.View);

        // Notify the viewer via SignalR
        await liveStreamHub.PerformActionMember(
            streamKey,
            viewerId,
            new LiveStreamBanInfo { Action = PerformActionEnum.Unkick, BanUntil = null, Reason = null });
    }

    private async Task MuteViewer(string streamKey, LiveStreamViewerDto viewer, int minutes, BlockReasonEnum reason)
    {
        var blackListItem = new BlackListItemDto
        {
            UserId = viewer.ViewerId,
            Username = viewer.ViewerName,
            Action = BlackListAction.Chat,
            BlockTo = DateTime.UtcNow.AddMinutes(minutes),
            Reason = reason,
        };
        liveStreamManager.AddBlackList(streamKey, blackListItem);

        // Notify the viewer via SignalR
        await liveStreamHub.PerformActionMember(
            streamKey,
            blackListItem.UserId,
            new LiveStreamBanInfo { Action = PerformActionEnum.Mute, BanUntil = blackListItem.BlockTo, Reason = reason });
    }

    private async Task UnmuteViewer(string streamKey, string viewerId)
    {
        liveStreamManager.RemoveBlackList(streamKey, viewerId, BlackListAction.Chat);

        // Notify the viewer via SignalR
        await liveStreamHub.PerformActionMember(
            streamKey,
            viewerId,
            new LiveStreamBanInfo { Action = PerformActionEnum.Unmute, BanUntil = null, Reason = null });
    }

    private async Task BlockDonation(string streamKey, LiveStreamViewerDto viewer, int minutes, BlockReasonEnum reason)
    {
        var blackListItem = new BlackListItemDto
        {
            UserId = viewer.ViewerId,
            Username = viewer.ViewerName,
            Action = BlackListAction.Donate,
            BlockTo = DateTime.UtcNow.AddMinutes(minutes),
            Reason = reason,
        };
        liveStreamManager.AddBlackList(streamKey, blackListItem);

        // Notify the viewer via SignalR
        await liveStreamHub.PerformActionMember(
            streamKey,
            blackListItem.UserId,
            new LiveStreamBanInfo { Action = PerformActionEnum.BlockDonation, BanUntil = blackListItem.BlockTo, Reason = reason });
    }

    private async Task UnblockDonation(string streamKey, string viewerId)
    {
        liveStreamManager.RemoveBlackList(streamKey, viewerId, BlackListAction.Donate);

        // Notify the viewer via SignalR
        await liveStreamHub.PerformActionMember(
            streamKey,
            viewerId,
            new LiveStreamBanInfo { Action = PerformActionEnum.UnblockDonation, BanUntil = null, Reason = null });
    }
}
