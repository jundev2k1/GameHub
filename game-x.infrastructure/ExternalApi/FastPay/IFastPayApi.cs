using game_x.share.ExternalApi.Base;
using game_x.share.ExternalApi.FastPay.Dtos.ApiRequests.Deposit;
using game_x.share.ExternalApi.FastPay.Dtos.ApiRequests.Withdrawal;
using Refit;

namespace game_x.infrastructure.ExternalApi.FastPay;

public interface IFastPayApi
{
    /// <summary>Fast Pay API: Merchant Member Usdt Deposit</summary>
    [Post("/v2/order/tron/usdt/deposit")]
    Task<ApiResponse<SecureResponse<FastPayDepositResponse>>> DepositAsync(
        [Body] SecureRequest<FastPayDepositRequest> request);

    /// <summary>Fast Pay API: Merchant Member Usdt Withdrawal</summary>
    [Post("/v1/order/tron/usdt/withdrawal")]
    Task<ApiResponse<SecureResponse<FastPayWithdrawalResponse>>> WithdrawalAsync(
        [Body] SecureRequest<FastPayWithdrawalRequest> request);
}
