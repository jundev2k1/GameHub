using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Rewards;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Transactions;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Features.Accounts.User.Dtos;
using game_x.infrastructure.SignalR.Facade;
using game_x.infrastructure.SignalR.Hubs;

namespace game_x.infrastructure.SignalR.Services;

public sealed class ClientHubService(ActorHubFacade<ClientHub, IClientHub> actorHub)
    : IClientHubService, IHubServices
{
    public async Task SendNotificationToMemberAsync(string memberId, NotificationDto dto)
    {
        await actorHub.Member(memberId).ReceiveNotification(dto);
    }

    public async Task SendTransactionToMemberAsync(string memberId, ClientTransactionDto transaction)
    {
        await actorHub.Member(memberId).TransactionUpdated(transaction);
    }
 
    public async Task SendVerifyUpdateAsync(string memberId, VerificationStatusDto verificationStatus)
    {
        await actorHub.Member(memberId).UserVerifyUpdated(verificationStatus);
    }

    public async Task SendWalletsToMemberAsync(string memberId, ClientWalletsDto wallets)
    {
        await actorHub.Member(memberId).WalletsUpdated(wallets);
    }

    public async Task NotifyWalletSynchronizationFailedAsync(string memberId, Guid platformId)
    {
        await actorHub.Member(memberId).NotifyWalletSynchronizationFailed(platformId);
    }
    
    public async Task SendTransactionTransferAsync(string memberId, TransactionTransferSignalDto transaction)
    {
        await actorHub.Member(memberId).TransactionTransfer(transaction);
    }
    
    public async Task SendRevokeRefreshTokenAsync(string memberId)
    {
        await actorHub.Member(memberId).RevokeRefreshToken(memberId);
    }
    
    public async Task SendInventoryAsync(string memberId, UserInventorySignalDto[] dto)
    {
        await actorHub.Member(memberId).InventoryUpdated(dto);
    }
}