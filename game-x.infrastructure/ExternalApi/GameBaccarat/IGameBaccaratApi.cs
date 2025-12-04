using game_x.share.ExternalApi.GameBaccarat.Dtos;
using game_x.share.ExternalApi.GameBaccarat.Dtos.Login;
using Refit;

namespace game_x.infrastructure.ExternalApi.GameBaccarat;

public interface IGameBaccaratApi
{
    /// <summary>Login API</summary>
    [Post("/partners/v1/auth/login")]
    Task<ApiResponse<ResponseBase<GameBaccaratLoginResponse>>> LoginAsync([Body] GameBaccaratLoginRequest request);
}
