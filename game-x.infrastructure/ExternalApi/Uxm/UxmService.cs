using game_x.application.Contract.Infrastructure.ExternalApi.Uxm;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Exceptions;
using game_x.share.ExternalApi.Uxm.Dtos;

namespace game_x.infrastructure.ExternalApi.Uxm;

public sealed class UxmService(IAppLogger<UxmService> logger, IUxmApi uxmApi) : IUxmService
{
    public async Task<SecureResponse<CreateOrderBuyResponseData>> CreateProxyBuyOrderAsync(
        SecureRequest<CreateOrderBuyRequestData> data)
    {
        try
        {
            // logger.LogInformation(
            //     $"Create order request: MerchantOrderId={data.Data.MerchantOrderId}, Amount={data.Data.FiatAmount}"
            //     );

            var response = await uxmApi.CreateProxyBuyOrderAsync(data);
            if (!response.IsSuccessStatusCode || response.Content == null)
            {
                logger.LogError($"Response failed: Status={response.StatusCode}");
                throw new ExternalServiceException();
            }

            logger.LogInformation($"Order created successfully: OrderUid={response.Content.Data.OrderUid}");
            return response.Content;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            throw;
        }
    }


}