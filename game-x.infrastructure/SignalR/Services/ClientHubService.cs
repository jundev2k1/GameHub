using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Features.Accounts.User.Dtos;
using game_x.infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace game_x.infrastructure.SignalR.Services;

public sealed class ClientHubService(IHubContext<ClientHub, IClientHub> hubContext)
    : IClientHubService, IHubServices
{
    public async Task SendNotificationToMemberAsync(string memberId, NotificationDto message)
    {
        await hubContext.Clients.Group($"member-{memberId}").ReceiveNotification(message);
    }

    public async Task SendTransactionToMemberAsync(string memberId, ClientTransactionDto transaction)
    {
        await hubContext.Clients.Group($"member-{memberId}").TransactionUpdated(transaction);
    }

    public async Task SendLedgerToMemberAsync(string userId, ClientLedgerDto ledger)
    {
        await hubContext.Clients.Group($"member-{userId}").LedgerUpdated(ledger);
    }

    public async Task SendVerifyUpdateAsync(string userId, VerificationStatusDto verificationStatus)
    {
        await hubContext.Clients.Group($"member-{userId}").UserVerifyUpdated(verificationStatus);
    }

    public async Task SendWalletsToMemberAsync(string userId, ClientWalletsDto wallets)
    {
        await hubContext.Clients.Group($"member-{userId}").WalletsUpdated(wallets);
    }
}
