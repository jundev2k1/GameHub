using game_x.application.Common.Abstractions.Events;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnUserBalanceUpdated;
using game_x.application.Exceptions;
using game_x.application.Features.Games.Dtos;
using game_x.application.Features.UserGameSessions.Dtos;
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
    IApplicationEventDispatcher eventDispatcher,
    IGameProviderCacheService gameProviderCache,
    IWalletManagerCacheService walletManagerCache,
    IUnitOfWork unitOfWork,
    IUserGameSessionRepo userGameSessionRepo,
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

        await CheckInAsync(userId, loginToken!);

        await Groups.AddToGroupAsync(Context.ConnectionId, $"games-{loginToken!.GamePlatformId}-{userId}");

        logger.LogInformation("GameHub ({PlatformId}) connected: {UserId}", loginToken.GamePlatformId, userId);
        await base.OnConnectedAsync();
    }

    private async Task CheckInAsync(string userId, GameHubTokenDto gameHubToken)
    {
        var connectionId = Context.ConnectionId;
        var (platformInfo, gameInfo) = GetGameInfo(gameHubToken);

        var currentSession = await userGameSessionRepo.GetCurrentSessionByUserIdAsync(userId, platformInfo.LocalId, gameInfo.LocalId);
        if (currentSession is null)
        {
            // Handle for new session
            var refreshEvent = new OnUserBalanceUpdatedEvent(userId, platformInfo.Id);
            await eventDispatcher.Publish(refreshEvent);
            var wallet = await walletManagerCache.GetWalletAsync(userId);
            var targetBalance = wallet.ExternalWallets.FirstOrDefault(ew => ew.PlatformId == gameHubToken.GamePlatformId)
                ?? throw new BadRequestException("Wallet was not found.");

            var userSession = UserGameSession.Create(userId, platformInfo.LocalId, gameInfo.LocalId, targetBalance.Amount);
            var connection = UserGameSessionConnection.Create(userSession.Id, connectionId);
            userSession.AddConnection(connection);
            await userGameSessionRepo.CreateAsync(userSession);
        }
        else
        {
            // Handle for creating connection
            var newConnection = UserGameSessionConnection.Create(currentSession.Id, connectionId);
            await userGameSessionRepo.AddConnectionAsync(newConnection);
        }

        await unitOfWork.SaveChangesAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? ex)
    {
        var userId = Context.UserIdentifier ?? string.Empty;
        if (userId.IsNotNullOrEmpty())
            logger.LogInformation("GameHub disconnected: {UserId}", userId);

        var token = httpContext.HttpContext?.Request.Query[TokenParamKey].FirstOrDefault();
        if (token.IsNullOrWhiteSpace())
            throw new ForbiddenException("Token is required.");

        if (!Base64Helper.TryDecode<GameHubTokenDto>(token!, out var loginToken))
            throw new BadRequestException();

        await CheckOutAsync(userId, loginToken!);

        await base.OnDisconnectedAsync(ex);
    }

    private async Task CheckOutAsync(string userId, GameHubTokenDto gameHubToken)
    {
        var connectionId = Context.ConnectionId;
        var (platformInfo, gameInfo) = GetGameInfo(gameHubToken);

        // Handle for new session
        var currentSession = await userGameSessionRepo.GetCurrentSessionByUserIdAsync(userId);
        if (currentSession is null)
            return;

        // Handle for reconnecting or creating connection
        var targetConnection = currentSession.Connections
            .FirstOrDefault(c => c.ConnectionId == connectionId);
        if (targetConnection is null)
            return;

        await userGameSessionRepo.UpdateConnectionAsync(targetConnection.Id, connection =>
        {
            connection.Disconnect();
        });
        await unitOfWork.SaveChangesAsync();
    }

    private (GamePlatformDto Platform, GameInfoDto Game) GetGameInfo(GameHubTokenDto gameHubToken)
    {
        var connectionId = Context.ConnectionId;
        var gameInfo = gameProviderCache.GameList.FirstOrDefault(gl =>
            gl.PlatformId == gameHubToken.GamePlatformId
            && gl.Id == gameHubToken.GameId)
            ?? throw new BadRequestException($"Game token ({gameHubToken.GamePlatformId} | {gameHubToken.GameId}) is invalid.");

        var platformInfo = gameProviderCache.PlatformList.FirstOrDefault(pl => pl.Id == gameHubToken.GamePlatformId)
            ?? throw new BadRequestException($"Game token ({gameHubToken.GamePlatformId} | {gameHubToken.GameId}) is invalid.");
        return (platformInfo, gameInfo);
    }
}
