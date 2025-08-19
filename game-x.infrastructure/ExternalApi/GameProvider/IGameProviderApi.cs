using game_x.share.ExternalApi.GameProvider.Dtos;
using Refit;

namespace game_x.infrastructure.ExternalApi.GameProvider;

public interface IGameProviderApi
{
    /// <summary>Login API</summary>
    [Post("/v1/login")]
    Task<ApiResponse<PayloadRequest>> LoginAsync(
        [Body] PayloadRequest request,
        [Header("Language")] string language,
        [Header("PIP")] string? ip = null);

    [Post("/v1/logout")]
    Task<ApiResponse<PayloadRequest>> LogoutAsync(
        [Body] PayloadRequest request,
        [Header("Language")] string language);

    /// <summary>Register API</summary>
    [Post("/v1/user/register")]
    Task<ApiResponse<PayloadRequest>> RegisterAsync(
        [Body] PayloadRequest request,
        [Header("Language")] string language);

    /// <summary>Get wallet API</summary>
    [Post("/v1/wallet")]
    Task<ApiResponse<PayloadRequest>> GetWalletAsync(
        [Body] PayloadRequest request,
        [Header("Language")] string language);

    [Post("/v1/wallet/deposit")]
    Task<ApiResponse<PayloadRequest>> DepositAsync(
        [Body] PayloadRequest request,
        [Header("Language")] string language);

    [Post("/v1/wallet/withdrawal")]
    Task<ApiResponse<PayloadRequest>> WithdrawalAsync(
        [Body] PayloadRequest request,
        [Header("Language")] string language);
}
