using game_x.share.ExternalApi.Uxm.Dtos;


namespace game_x.application.Contract.Infrastructure.ExternalApi.Uxm;

public interface IUxmService
{
    Task<SecureResponse<UxmDepositOrderResponseData>> CreateDepositOrderAsync(
        SecureRequest<UxmDepositOrderRequestData> data);

    Task<SecureResponse<UxmWithdrawalOrderResponseData>> CreateWithdrawalOrderAsync(
        SecureRequest<UxmWithdrawalOrderRequest> data);
}
