using game_x.share.ExternalApi.SasSlot.Dtos.Login;
using Refit;

namespace game_x.infrastructure.ExternalApi.SasSlot;

public interface ISasSlotApi
{
    /// <summary>Login API</summary>
    [Post("/api/session")]
    Task<ApiResponse<SasSlotLoginResponse>> LoginAsync([Body] SasSlotLoginRequest request);
}
