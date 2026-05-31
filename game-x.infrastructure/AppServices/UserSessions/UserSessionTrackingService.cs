using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Events;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.Account.OnUserBalanceUpdated;
using game_x.application.Exceptions;
using game_x.application.Features.Games.Dtos;
using game_x.application.Features.UserGameSessions.Services;

namespace game_x.infrastructure.AppServices.UserSessions;

public sealed class UserSessionTrackingService(
    IUnitOfWork unitOfWork,
    IUserGameSessionRepo userSessionRepo,
    IWalletManagerCacheService walletManagerCache,
    IGameProviderCacheService gameProviderCache,
    IApplicationEventDispatcher eventDispatcher) : IUserSessionTrackingService, IServices
{
    public async Task CheckInAsync(
        string userId,
        string connectionId,
        Guid platformId,
        Guid? gameId, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            var (platformInfo, gameInfo) = GetGameInfo(platformId, gameId);
            var currentSession = await userSessionRepo.GetCurrentSessionByUserIdAsync(
                userId,
                platformInfo.LocalId,
                gameInfo?.LocalId,
                ct);
            if (currentSession is null)
            {
                // Handle for new session
                var refreshEvent = new OnUserBalanceUpdatedEvent(userId, platformInfo.Id);
                await eventDispatcher.Publish(refreshEvent);
                var wallet = await walletManagerCache.GetWalletAsync(userId);
                var targetBalance = wallet.ExternalWallets.FirstOrDefault(ew => ew.PlatformId == platformId)
                    ?? throw new BadRequestException("Wallet was not found.");

                // Handle for new session
                var userSession = UserGameSession.Create(userId, platformInfo.LocalId, gameInfo?.LocalId, targetBalance.Amount);
                var connection = UserGameSessionConnection.Create(userSession.Id, connectionId);
                userSession.RegisterConnection(connection);

                await userSessionRepo.CreateAsync(userSession, ct);
            }
            else
            {
                // Handle for creating connection
                var newConnection = UserGameSessionConnection.Create(currentSession.Id, connectionId);
                await userSessionRepo.RegisterConnectionAsync(newConnection, ct);
            }
        }, ct: ct);
    }

    public async Task<bool> PingAsync(string connectionId, CancellationToken ct = default)
    {
        return await userSessionRepo.PingAsync(connectionId, ct);
    }

    public async Task CheckOutAsync(string connectionId, CancellationToken ct = default)
    {
        await userSessionRepo.CheckOutAsync(connectionId, ct);
    }

    private (GamePlatformDto Platform, GameInfoDto Game) GetGameInfo(Guid platformId, Guid? gameId)
    {
        var gameInfo = gameProviderCache.GameList.FirstOrDefault(gl =>
            gl.PlatformId == platformId
            && gl.Id == gameId)
            ?? throw new BadRequestException($"Game token ({platformId} | {gameId}) is invalid.");

        var platformInfo = gameProviderCache.PlatformList.FirstOrDefault(pl => pl.Id == platformId)
            ?? throw new BadRequestException($"Game token ({platformId} | {gameId}) is invalid.");
        return (platformInfo, gameInfo);
    }
}
