using game_x.share.ExternalApi.SasSlot.Dtos.Login;
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
}
