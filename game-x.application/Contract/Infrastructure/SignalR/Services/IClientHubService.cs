using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Transactions;
using game_x.application.Features.Accounts.User.Dtos;

namespace game_x.application.Contract.Infrastructure.SignalR.Services;

public interface IClientHubService
{
    Task   SendNotificationToMemberAsync(string memberId, NotificationDto dto);
    Task SendTransactionToMemberAsync(string memberId, ClientTransactionDto transaction);
    Task SendVerifyUpdateAsync(string userId, VerificationStatusDto verificationStatus);
    Task SendWalletsToMemberAsync(string userId, ClientWalletsDto wallets);
    Task NotifyWalletSynchronizationFailedAsync(string userId, Guid platformId);
    Task SendTransactionTransferAsync(string memberId, TransactionTransferSignalDto transaction);
}