using game_x.share.ExternalApi.FastPay.Dtos.ApiRequests.Deposit;
using game_x.share.ExternalApi.FastPay.Dtos.ApiRequests.Withdrawal;

namespace game_x.application.Contract.Infrastructure.ExternalApi.FastPay;

public interface IFastPayService
{
    Task<FastPayDepositResponse> DepositAsync(
        decimal amount,
        string orderNumber,
        string userId,
        string remark);

    Task<FastPayWithdrawalResponse> WithdrawalAsync(
        decimal amount,
        string orderNumber,
        string to,
        string? remark);
}