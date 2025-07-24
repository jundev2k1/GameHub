using game_x.share.ExternalApi.Uxm.Dtos;

namespace game_x.application.Contract.Infrastructure.ExternalApi.Uxm;

public interface IUxmService
{
    Task<SecureResponse<CreateOrderBuyResponseData>> CreateProxyBuyOrderAsync(
        SecureRequest<CreateOrderBuyRequestData> data);

    Task<SecureResponse<CreateOrderSellResponseData>> CreateProxySellOrderAsync(
        SecureRequest<CreateOrderSellRequestData> data);

    Task<CreateBuyOrderV2Response> CreateProxyBuyOrderV2Async(CreateBuyOrderV2Request data);
    
    Task<SecureResponse<CreateSellOrderV2Response>> CreateProxySellOrderV2Async(
        SecureRequest<CreateSellOrderV2Request> data);
    
    Task<SecureResponse<UxmOrderDetailInfoResponse>> GetOrderDetailInfoAsync(
        SecureRequest<GetUxmOrderDetailInfoRequest> data);
    
    Task<SecureResponse<EstimateQuoteResponse>> GetEstimateQuoteAsync(
        SecureRequest<EstimateQuoteRequest> data);
}
