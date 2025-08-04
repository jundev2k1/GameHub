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
            logger.LogInformation("Send withdrawal request to UXM: to = {To}, amount = {Amount}, order = {OtcOrderNumber}", data.Data.To, data.Data.Amount, data.Data.OtcOrderNumber);
     
            var response = await uxmApi.CreateProxyWithdrawalOrderAsync(data);
            if (!response.IsSuccessStatusCode || response.Content == null)
            {
                logger.LogError($"Response failed: Status={response.StatusCode}");
                throw new ExternalServiceException();
            }
            logger.LogInformation("Withdrawal request successful，order: {order}", data.Data.OtcOrderNumber);
            return response.Content;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to send withdrawal request to UXM: {Ex}", ex);
            throw new BadRequestException(MessageCode.System.DependencyFailure);
        }
    }
}
