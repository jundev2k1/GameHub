using game_x.share.ExternalApi.Uxm.Dtos;


namespace game_x.application.Contract.Infrastructure.ExternalApi.Uxm;

public interface IUxmService
{
    Task<SecureResponse<CreateOrderBuyResponseData>> CreateProxyBuyOrderAsync(
        SecureRequest<CreateOrderBuyRequestData> data);

}
