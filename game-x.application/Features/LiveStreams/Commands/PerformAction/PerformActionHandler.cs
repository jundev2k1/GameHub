using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.Srs;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.LiveStreams.Dtos;
using game_x.application.Features.LiveStreams.Enum;

namespace game_x.application.Features.LiveStreams.Commands.PerformAction;

public sealed class PerformActionHandler(
    IUserAccessor userAccessor,
    ILiveStreamRepo liveStreamRepo,
    ILiveStreamManagerCacheService liveStreamManager,
    ISrsService srsService) : ICommandHandler<PerformActionCommand>
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
                UnkickViewer(targetSchedule.StreamKey, request.ViewerId!);
                break;

            case PerformActionEnum.Mute:
                MuteViewer(targetSchedule.StreamKey, targetViewer, request.BlockTime!.Value, request.Reason!.Value);
                break;

            case PerformActionEnum.Unmute:
                UnmuteViewer(targetSchedule.StreamKey, request.ViewerId!);
                break;

            case PerformActionEnum.BlockDonation:
                BlockDonation(targetSchedule.StreamKey, targetViewer, request.BlockTime!.Value, request.Reason!.Value);
                break;

            case PerformActionEnum.UnblockDonation:
                UnblockDonation(targetSchedule.StreamKey, request.ViewerId!);
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
    }

    private void UnkickViewer(string streamKey, string viewerId)
    {
        liveStreamManager.RemoveBlackList(streamKey, viewerId, BlackListAction.View);
    }

    private void MuteViewer(string streamKey, LiveStreamViewerDto viewer, int minutes, BlockReasonEnum reason)
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
    }

    private void UnmuteViewer(string streamKey, string viewerId)
    {
        liveStreamManager.RemoveBlackList(streamKey, viewerId, BlackListAction.Chat);
    }

    private void BlockDonation(string streamKey, LiveStreamViewerDto viewer, int minutes, BlockReasonEnum reason)
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
    }

    private void UnblockDonation(string streamKey, string viewerId)
    {
        liveStreamManager.RemoveBlackList(streamKey, viewerId, BlackListAction.Donate);
    }
}
