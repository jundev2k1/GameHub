using game_x.share.ExternalApi.GameProvider.Dtos;
using game_x.share.ExternalApi.GameProvider.Dtos.Login;
using Refit;

namespace game_x.infrastructure.ExternalApi.GameProvider;

public interface IGameProviderApi
{
    /// <summary>Login API</summary>
    [Post("/v1/login")]
    Task<ApiResponse<LoginResponse>> LoginAsync(
    [Body] PayloadRequest request,
    [Header("Language")] string language,
    [Header("PIP")] string? ip = null);
}
