using game_x.application.Contract.Infrastructure.ExternalApi.Uxm;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Exceptions;
using game_x.share.ExternalApi.Uxm.Dtos;

namespace game_x.infrastructure.ExternalApi.Uxm;

public sealed class UxmService(IAppLogger<UxmService> logger, IUxmApi uxmApi) : IUxmService
{
    public async Task<SecureResponse<UxmWithdrawalOrderResponseData>> CreateWithdrawalOrderAsync(
      SecureRequest<UxmWithdrawalOrderRequest> data)
    {
        try
        {
            logger.LogInformation($"Send withdrawal request to UXM: MerchantNumber={{MerchantNumber}}, To = {{To}}, Amount = {{Amount}}, OrderNumber = {{OtcOrderNumber}}", 
                data.Data.MerchantNumber, 
                data.Data.To,
                data.Data.Amount, 
                data.Data.OrderNumber);
     
            var response = await uxmApi.CreateProxyWithdrawalOrderAsync(data);
            if (!response.IsSuccessStatusCode || response.Content == null)
            {
                logger.LogError($"Response failed: Status={response.StatusCode}, Message={response.Error?.Message}");
                throw new ExternalServiceException();
            }
            logger.LogInformation("Withdrawal request successful，OrderUid: {{OrderUid}}", response.Content.Data.OrderUid!);
            return response.Content;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send withdrawal request to UXM: {Ex}", ex);
            throw new BadRequestException(MessageCode.System.DependencyFailure);
        }
    }

    public async Task<SecureResponse<UxmDepositOrderResponseData>> CreateDepositOrderAsync(
        SecureRequest<UxmDepositOrderRequest> data)
    {
        try
        {
            logger.LogInformation($"Send deposit request to UXM: MerchantNumber={{MerchantNumber}}, UserId={{UserId}}, Amount = {{Amount}}, OrderNumber = {{OtcOrderNumber}}", 
                data.Data.MerchantNumber, 
                data.Data.UserId, 
                data.Data.Amount, 
                data.Data.OrderNumber);

            var response = await uxmApi.CreateProxyDepositOrderAsync(data);
            if (!response.IsSuccessStatusCode || response.Content == null)
            {
                logger.LogError($"Response failed: Status={response.StatusCode}, Message={response.Error?.Message}");
                throw new ExternalServiceException();
            }
            logger.LogInformation("Deposit request successful，OrderUid: {{OrderUid}}", response.Content.Data.OrderUid);
            return response.Content;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send deposit request to UXM: {Ex}", ex);
            throw;
        }
    }
}