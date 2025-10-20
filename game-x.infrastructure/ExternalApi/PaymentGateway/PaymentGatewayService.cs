using game_x.application.Contract.Infrastructure.ExternalApi.PaymentGateway;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Exceptions;
using game_x.application.Features.ChainTransactions.Mapping;
using game_x.infrastructure.Security.Asymmetric;
using game_x.share.ExternalApi.PaymentGateway.Dtos;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.infrastructure.ExternalApi.PaymentGateway;

public class PaymentGatewayService(
    IAppLogger<PaymentGatewayService> logger, 
    IPaymentGatewayApi api,
    IOptions<GameXSettings> gameXSettings): IPaymentGatewayService
{
    public async Task<SecureResponse<OrderResponse>> ProxyDepositAsync(SecureRequest<DepositOrderRequest> data)
    {
        try
        {
            var apiKey = gameXSettings.Value.PaymentGatewayApiKey;
            
            logger.LogInformation(
                $"Create order request: ProviderId={data.Data.ProviderId}, Amount={data.Data.Amount}, OrderNumber={data.Data.OrderNumber}");

            var payload = data.Data.Adapt<DepositOrderPayload>() with{Signature = data.Signature};
            var response = await api.ProxyDepositOrderAsync(apiKey, payload);
            if (!response.IsSuccessStatusCode || response.Content == null)
            {
                logger.LogError($"Response failed: Status={response.StatusCode}");
                throw new ExternalServiceException();
            }

            logger.LogInformation($"Order created successfully: OrderUid={response.Content.Data.TransactionId}");
            return response.Content;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            throw;
        }
    }

    public async Task<SecureResponse<OrderResponse>> ProxyWithdrawalAsync(SecureRequest<WithdrawalOrderRequest> data)
    {
        try
        {
            var apiKey = gameXSettings.Value.PaymentGatewayApiKey;
            
            logger.LogInformation(
                $"Create order request: ProviderId={data.Data.ProviderId}, Amount={data.Data.Amount}, OrderNumber={data.Data.OrderNumber}");

            var payload = data.Data.Adapt<WithdrawalOrderPayload>() with{Signature = data.Signature};
            var response = await api.ProxyWithdrawalOrderAsync(apiKey, payload);
            if (!response.IsSuccessStatusCode || response.Content == null)
            {
                logger.LogError($"Response failed: Status={response.StatusCode}");
                throw new ExternalServiceException();
            }

            logger.LogInformation($"Order created successfully: OrderUid={response.Content.Data.TransactionId}");
            return response.Content;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            throw;
        }
    }
}