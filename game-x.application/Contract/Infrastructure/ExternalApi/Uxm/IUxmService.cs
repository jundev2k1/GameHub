using game_x.share.ExternalApi.Base;
using game_x.share.ExternalApi.Uxm.Dtos.ApiRequests.Deposit;
using game_x.share.ExternalApi.Uxm.Dtos.ApiRequests.Withdrawal;

namespace game_x.application.Contract.Infrastructure.ExternalApi.Uxm;

public interface IUxmService
{
    Task<UxmDepositResponse> DepositAsync(
        decimal amount,
        string orderNumber,
        string userId,
        string remark);

    Task<UxmWithdrawalResponse> WithdrawalAsync(
        decimal amount,
        string orderNumber,
        string to,
        string? remark);
}