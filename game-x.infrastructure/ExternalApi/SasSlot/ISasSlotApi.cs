using game_x.share.ExternalApi.SasSlot.Dtos.Deposit;
using game_x.share.ExternalApi.SasSlot.Dtos.Login;
using game_x.share.ExternalApi.SasSlot.Dtos.Withdrawal;
using Refit;

namespace game_x.infrastructure.ExternalApi.SasSlot;

public interface ISasSlotApi
{
    /// <summary>Login API</summary>
    [Post("/ext/session")]
    Task<ApiResponse<SasSlotLoginResponse>> LoginAsync(
        [Body] SasSlotLoginRequest request,
        [Header("X-Signature")] string signature,
        [Header("X-Signature-Alg")] string signatureAlg,
        [Header("X-Key-Id")] string keyId);

    /// <summary>Deposit API</summary>
    [Post("/ext/deposit")]
    Task<ApiResponse<SasSlotDepositResponse>> DepositAsync(
        [Body] SasSlotDepositRequest request,
        [Header("X-Signature")] string signature,
        [Header("X-Signature-Alg")] string signatureAlg,
        [Header("X-Key-Id")] string keyId);

    /// <summary>Withdrawal API</summary>
    [Post("/ext/withdrawal")]
    Task<ApiResponse<SasSlotWithdrawalResponse>> WithdrawalAsync(
        [Body] SasSlotWithdrawalRequest request,
        [Header("X-Signature")] string signature,
        [Header("X-Signature-Alg")] string signatureAlg,
        [Header("X-Key-Id")] string keyId);
}
