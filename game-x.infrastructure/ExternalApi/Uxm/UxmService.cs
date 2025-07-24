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
            logger.LogInformation(
                $"Create order request: MerchantOrderId={data.Data.MerchantOrderId}, Amount={data.Data.FiatAmount}");

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

    public async Task<SecureResponse<CreateOrderSellResponseData>> CreateProxySellOrderAsync(
        SecureRequest<CreateOrderSellRequestData> data)
    {
        try
        {
            logger.LogInformation(
                $"Create order request: MerchantOrderId={data.Data.MerchantOrderId}, Amount={data.Data.FiatAmount}");

            var response = await uxmApi.CreateProxySellOrderAsync(data);
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

    public async Task<CreateBuyOrderV2Response> CreateProxyBuyOrderV2Async(CreateBuyOrderV2Request data)
    {
        try
        {
            logger.LogInformation(
                $"Create order request: MerchantOrderId={data.Data.MerchantOrderId}, Amount={data.Data.Amount}");

            var response = await uxmApi.CreateProxyBuyOrderV2Async(data.Signature, data.Data);

            if (!response.IsSuccessStatusCode || response.Content == null)
            {
                logger.LogError($"Response failed: Status={response.StatusCode}, Error={response.Error}");
                throw new ExternalServiceException();
            }

            var signature = response.Headers.GetValues("signature").FirstOrDefault();
            if (string.IsNullOrEmpty(signature))
                throw new ExternalServiceException();

            logger.LogInformation($"Order created successfully: OrderUid={response.Content.OrderUid}");

            return new CreateBuyOrderV2Response
            {
                Data = response.Content,
                Signature = signature
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            throw;
        }
    }
    
    public async Task<SecureResponse<CreateSellOrderV2Response>> CreateProxySellOrderV2Async(
        SecureRequest<CreateSellOrderV2Request> data)
    {
        try
        {
            logger.LogInformation(
                $"Create order request: MerchantOrderId={data.Data.MerchantOrderId}, Amount={data.Data.Amount}");
    
            var response = await uxmApi.CreateProxySellOrderV2Async(data.Signature, data.Data);
            if (!response.IsSuccessStatusCode || response.Content == null)
            {
                logger.LogError($"Response failed: Status={response.StatusCode}");
                throw new ExternalServiceException();
            }
            
            var signature = response.Headers.GetValues("signature").FirstOrDefault();
            if (string.IsNullOrEmpty(signature))
                throw new ExternalServiceException();
    
            logger.LogInformation($"Order created successfully: OrderUid={response.Content.OrderUid}");
    
            return new SecureResponse<CreateSellOrderV2Response>(response.Content, signature);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            throw;
        }
    }
    
    public async Task<SecureResponse<UxmOrderDetailInfoResponse>> GetOrderDetailInfoAsync(
        SecureRequest<GetUxmOrderDetailInfoRequest> data)
    {
        try
        {
            logger.LogInformation(
                $"Get order request: UxmOrderId={data.Data.TradeNo}, Merchant={data.Data.MerchantNumber}");
    
            var response = await uxmApi.GetOrderDetailInfoAsync(
                tradeNo: data.Data.TradeNo, 
                merchantNumber: data.Data.MerchantNumber, 
                timestamp: data.Data.Timestamp,
                signature: data.Signature);
            
            if (!response.IsSuccessStatusCode || response.Content == null)
            {
                logger.LogError($"Response failed: Status={response.StatusCode}");
                throw new ExternalServiceException();
            }
            
            var signature = response.Headers.GetValues("signature").FirstOrDefault();
            if (string.IsNullOrEmpty(signature))
                throw new ExternalServiceException();
    
            logger.LogInformation($"Get order's information successfully: OrderUid={data.Data.TradeNo}");
    
            return new SecureResponse<UxmOrderDetailInfoResponse>(response.Content, signature);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            throw;
        }
    }

    public async Task<SecureResponse<EstimateQuoteResponse>> GetEstimateQuoteAsync(
        SecureRequest<EstimateQuoteRequest> data)
    {
        try
        {
            logger.LogInformation(
                $"Get estimate quote request: Merchant={data.Data.MerchantNumber}");
    
            var response = await uxmApi.GetEstimateQuoteAsync(
                data: data.Data,
                signature: data.Signature);
            
            if (!response.IsSuccessStatusCode || response.Content == null)
            {
                logger.LogError($"Response failed: Status={response.StatusCode}");
                throw new ExternalServiceException();
            }
            
            var signature = response.Headers.GetValues("signature").FirstOrDefault();
            if (string.IsNullOrEmpty(signature))
                throw new ExternalServiceException();
    
            logger.LogInformation($"Get estimate quote successfully");
    
            return new SecureResponse<EstimateQuoteResponse>(response.Content, signature);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            throw new ExternalServiceException();
        }
    }
}