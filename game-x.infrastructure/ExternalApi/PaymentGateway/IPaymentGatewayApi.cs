using game_x.share.ExternalApi.PaymentGateway.Dtos;
using Refit;

namespace game_x.infrastructure.ExternalApi.PaymentGateway;

public interface IPaymentGatewayApi
{
    /// <summary>Create a new deposit order.</summary>
    [Post("/api/merchant/deposit")]
    Task<ApiResponse<SecureResponse<OrderResponse>>> ProxyDepositOrderAsync([Header("X-Api-Key")] string apiKey, [Body] DepositOrderPayload request);

    /// <summary>Create a new withdrawal order.</summary>
    [Post("/api/merchant/withdrawal")]
    Task<ApiResponse<SecureResponse<OrderResponse>>> ProxyWithdrawalOrderAsync([Header("X-Api-Key")] string apiKey, [Body] WithdrawalOrderPayload request);
}