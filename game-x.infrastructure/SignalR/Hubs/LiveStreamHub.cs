using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Exceptions;
using game_x.application.Features.LiveStreams.Commands.PerformAction;
using game_x.application.Features.LiveStreams.Dtos;
using game_x.application.Features.LiveStreams.Enum;
using game_x.application.Features.LiveStreams.Queries.GetViewersByStream;
using game_x.share.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace game_x.infrastructure.SignalR.Hubs;

public interface ILiveStreamHub
{
    Task NotifyMessageFailed(MessageFailedSignalDto signalDto);

    Task OnMemberAction();

    Task OnStreamReconnected();

    Task OnStreamDisconnected();

    Task OnStreamCanceled(BlockReasonEnum reason);

    Task ReceiveViewerBatch(LiveStreamViewerInfoDto[] viewers);

    Task OnViewerBatchComplete();
}

[Authorize(Roles = AppRoles.User)]
public sealed class LiveStreamHub(
    ISender sender,
    ILiveStreamManagerCacheService liveStreamManager,
    IHttpContextAccessor httpContext,
    IAppLogger<LiveStreamHub> logger) : Hub<ILiveStreamHub>
{
    public const string Path = "/hubs/live-stream";

    private const string StreamKeyParamKey = "stream-key";

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier
            ?? throw new ForbiddenException("Token invalid.");
        var streamKey = httpContext.HttpContext?.Request.Query[StreamKeyParamKey].FirstOrDefault();
        if (streamKey.IsNullOrWhiteSpace())
            throw new ForbiddenException("Stream key is required.");

        var streamInfo = liveStreamManager.GetLiveStreamStatus(streamKey!)
            ?? throw new ForbiddenException("Stream not found.");

        await Groups.AddToGroupAsync(Context.ConnectionId, $"stream-{streamKey}");
        await Groups.AddToGroupAsync(Context.ConnectionId, $"stream-{streamKey}-member-{userId}");

        if (userId == streamInfo.AssignedTo?.Id)
            await Groups.AddToGroupAsync(Context.ConnectionId, $"stream-{streamKey}-host-{userId}");

        logger.LogInformation("LiveStreamHub {StreamKey} connected: {UserId}", streamKey!, userId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? ex)
    {
        var userId = Context.UserIdentifier ?? string.Empty;
        if (userId.IsNotNullOrEmpty())
            logger.LogInformation("LiveStreamHub disconnected: {UserId}", userId);

        await base.OnDisconnectedAsync(ex);
    }

    public async Task PerformActionAsync(PerformActionCommand command)
    {
        var streamKey = httpContext.HttpContext?.Request.Query[StreamKeyParamKey].FirstOrDefault();
        if (streamKey.IsNullOrWhiteSpace())
            throw new ForbiddenException("Stream key is required.");

        await sender.Send(command with { StreamKey = streamKey });
    }

    public async Task StreamAllViewers()
    {
        var streamKey = httpContext.HttpContext?.Request.Query[StreamKeyParamKey].FirstOrDefault();
        if (streamKey.IsNullOrWhiteSpace())
            throw new ForbiddenException("Stream key is required.");

        var query = new GetViewersByStreamQuery(streamKey!);
        var allViewers = await sender.Send(query);
        foreach (var viewers in allViewers.Chunk(250))
        {
            await Clients.Caller.ReceiveViewerBatch(viewers);
        }

        await Clients.Caller.OnViewerBatchComplete();
    }
}
