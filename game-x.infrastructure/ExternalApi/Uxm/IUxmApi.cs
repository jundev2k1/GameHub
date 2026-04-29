using game_x.share.ExternalApi.Base;
using game_x.share.ExternalApi.Uxm.Dtos.ApiRequests.Deposit;
using game_x.share.ExternalApi.Uxm.Dtos.ApiRequests.Withdrawal;
using Refit;

namespace game_x.infrastructure.ExternalApi.Uxm;

public interface IUxmApi
{
    /// <summary>Uxm API: Merchant Member Usdt Deposit</summary>
    [Post("/v2/order/tron/usdt/deposit")]
    Task<ApiResponse<SecureResponse<UxmDepositResponse>>> DepositAsync(
        [Body] SecureRequest<UxmDepositRequest> request);

    /// <summary>Uxm API: Merchant Member Usdt Withdrawal</summary>
    [Post("/v1/order/tron/usdt/withdrawal")]
    Task<ApiResponse<SecureResponse<UxmWithdrawalResponse>>> WithdrawalAsync(
        [Body] SecureRequest<UxmWithdrawalRequest> request);
}
