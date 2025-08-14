using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Features.ChainTransactions.Dtos;
using game_x.application.Features.UserWallet.Dtos;

namespace game_x.application.Contract.Infrastructure.SignalR.Services;

public interface IClientHubService
{
    Task SendNotificationToMemberAsync(string memberId, NotificationDto message);
    Task SendTransactionToMemberAsync(string memberId, ClientTransactionDto transaction);
    Task SendBalanceToMemberAsync(string userId, ClientBalanceDto balances);
    Task SendLedgerToMemberAsync(string userId, ClientLedgerDto ledger);
    Task SendUserKcyToMemberAsync(string userId, UserKycDto userKyc);
    Task SendUserBankAccountToMemberAsync(string userId, UserBankAccountDto userBankAccount);
}
