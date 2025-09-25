using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.LiveStream;
using game_x.application.Exceptions;
using game_x.application.Features.LiveStreams.Streaming.Commands.DeleteChatMessage;
using game_x.application.Features.LiveStreams.Streaming.Commands.EndStream;
using game_x.application.Features.LiveStreams.Streaming.Commands.PerformAction;
using game_x.application.Features.LiveStreams.Streaming.Commands.SendChatMessage;
using game_x.application.Features.LiveStreams.Streaming.Dtos;
using game_x.application.Features.LiveStreams.Streaming.Queries.GetViewersByStream;
using game_x.share.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace game_x.infrastructure.SignalR.Hubs;

public interface ILiveStreamHub
{
    Task NotifyMessageFailed(string MessageId);

    Task OnMemberAction(LiveStreamBanInfo banInfo);

    Task OnStreamReconnected();

    Task OnStreamDisconnected();

    Task OnStreamCanceled(string reason);

    Task OnStreamEnded();

    Task OnUserJoined(LiveStreamViewerInfoDto viewer);

    Task OnUserLeft(string viewerId);

    Task OnViewChange(int viewCount);

    Task ReceiveViewerBatch(LiveStreamViewerInfoDto[] viewers);

    Task OnViewerBatchComplete();

    Task OnReceiveMessage(LiveStreamChatMessageDto viewer);

    Task OnMessageDeleted(Guid messageId);
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

        // Add to groups, one for all members, one for the specific member
        await Groups.AddToGroupAsync(Context.ConnectionId, $"stream-{streamKey}");
        await Groups.AddToGroupAsync(Context.ConnectionId, $"stream-{streamKey}-member-{userId}");

        // If the user is the host, add to host group
        if (userId == streamInfo.AssignedTo?.Id)
            await Groups.AddToGroupAsync(Context.ConnectionId, $"stream-{streamKey}-host");

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

    public async Task EndStream()
    {
        var streamKey = httpContext.HttpContext?.Request.Query[StreamKeyParamKey].FirstOrDefault();
        if (streamKey.IsNullOrWhiteSpace())
            throw new ForbiddenException("Stream key is required.");

        var command = new EndStreamCommand(streamKey!);
        await sender.Send(command);
    }

    public async Task SendChatMessage(LiveStreamChatMessageInputDto input)
    {
        var streamKey = httpContext.HttpContext?.Request.Query[StreamKeyParamKey].FirstOrDefault();
        if (streamKey.IsNullOrWhiteSpace())
            throw new ForbiddenException("Stream key is required.");

        var command = new SendChatMessageCommand(streamKey!, input.Id, input.Message);
        await sender.Send(command);
    }

    public async Task DeleteChatMessage(Guid messageId)
    {
        var streamKey = httpContext.HttpContext?.Request.Query[StreamKeyParamKey].FirstOrDefault();
        if (streamKey.IsNullOrWhiteSpace())
            throw new ForbiddenException("Stream key is required.");

        var command = new DeleteChatMessageCommand(streamKey!, messageId);
        await sender.Send(command);
    }
}
