using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Exceptions;
using game_x.application.Features.UserGameSessions.Dtos;
using game_x.application.Features.UserGameSessions.Services;
using game_x.share.Extensions;
using game_x.share.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace game_x.infrastructure.SignalR.Hubs;

public interface IGameHub
{
}

[Authorize(Roles = AppRoles.User)]
public sealed class GameHub(
    IHttpContextAccessor httpContext,
    IUserSessionTrackingService sessionTrackingService,
    IAppLogger<GameHub> logger) : Hub<IGameHub>
{
    public const string Path = "/hubs/games";

    private const string TokenParamKey = "token";

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier
            ?? throw new ForbiddenException("Token invalid.");
        var token = httpContext.HttpContext?.Request.Query[TokenParamKey].FirstOrDefault();
        if (token.IsNullOrWhiteSpace())
            throw new ForbiddenException("Token is required.");

        if (!Base64Helper.TryDecode<GameHubTokenDto>(token!, out var loginToken))
            throw new BadRequestException();

        await sessionTrackingService.CheckInAsync(
            userId,
            Context.ConnectionId,
            loginToken!.GamePlatformId,
            loginToken.GameId);

        await Groups.AddToGroupAsync(Context.ConnectionId, $"games-{loginToken!.GamePlatformId}-{userId}");

        logger.LogInformation("GameHub ({PlatformId}) connected: {UserId}", loginToken.GamePlatformId, userId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? ex)
    {
        var userId = Context.UserIdentifier ?? string.Empty;
        if (userId.IsNotNullOrEmpty())
            logger.LogInformation("GameHub disconnected: {UserId}", userId);

        await sessionTrackingService.CheckOutAsync(Context.ConnectionId);

        await base.OnDisconnectedAsync(ex);
    }

    public async Task<bool> Ping()
    {
        return await sessionTrackingService.PingAsync(Context.ConnectionId);
    }
}
