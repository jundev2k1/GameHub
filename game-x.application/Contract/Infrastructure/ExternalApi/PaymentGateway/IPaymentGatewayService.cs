using game_x.share.ExternalApi.PaymentGateway.Dtos;

namespace game_x.application.Contract.Infrastructure.ExternalApi.PaymentGateway;

public interface IPaymentGatewayService
{
    Task<SecureResponse<OrderResponse>> ProxyDepositAsync(SecureRequest<DepositOrderRequest> data);
    Task<SecureResponse<OrderResponse>> ProxyWithdrawalAsync(SecureRequest<WithdrawalOrderRequest> data);
}