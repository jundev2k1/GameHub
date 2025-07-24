using game_x.share.ExternalApi.Uxm.Dtos;
using Refit;

namespace game_x.infrastructure.ExternalApi.Uxm;

public interface IUxmApi
{
    /// <summary>Uxm API: 商戶會員法幣儲值</summary>
    /// <summary>Uxm API: Merchant Member Fiat Top-up</summary>
    [Post("/v1/order/payment-di")]
    Task<ApiResponse<SecureResponse<CreateOrderBuyResponseData>>> CreateProxyBuyOrderAsync(
        [Body] SecureRequest<CreateOrderBuyRequestData> request);

    /// <summary>Uxm API: 商戶會員法幣出金</summary>
    [Post("/v1/order/payment-do")]
    Task<ApiResponse<SecureResponse<CreateOrderSellResponseData>>> CreateProxySellOrderAsync(
        [Body] SecureRequest<CreateOrderSellRequestData> request);

    /// <summary>Uxm API: Merchant Member Fiat Top-up version 2</summary>
    [Post("/v2/order/payment-di")]
    Task<ApiResponse<CreateBuyOrderV2ResponseData>> CreateProxyBuyOrderV2Async([Header("Signature")] string signature,
        [Body] CreateBuyOrderV2ReqData data);
    
    /// <summary>Uxm API: Merchant Member Fiat withdraw version 2</summary>
    [Post("/v2/order/payment-do")]
    Task<ApiResponse<CreateSellOrderV2Response>> CreateProxySellOrderV2Async([Header("Signature")] string signature,
        [Body] CreateSellOrderV2Request data);
    
    /// <summary>Uxm API: get order's detail information</summary>
    [Get("/v1/s2s/order/{tradeNo}")]
    Task<ApiResponse<UxmOrderDetailInfoResponse>> GetOrderDetailInfoAsync(
        [AliasAs("tradeNo")] string tradeNo,
        [Query] string merchantNumber,
        [Query] long timestamp,
        [Header("Signature")] string signature);
    
    /// <summary>Uxm API: get estimate quote</summary>
    [Post("/v1/s2s/estimate-quote")]
    Task<ApiResponse<EstimateQuoteResponse>> GetEstimateQuoteAsync(
        [Header("Signature")] string signature,
        [Body] EstimateQuoteRequest data);
}