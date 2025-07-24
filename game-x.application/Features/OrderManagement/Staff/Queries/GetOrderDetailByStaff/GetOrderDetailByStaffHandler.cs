using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.Uxm;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.OrderManagement.Dtos;
using game_x.application.Mappers.Order;
using game_x.share.Extensions;
using game_x.share.ExternalApi.Uxm.Dtos;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.application.Features.OrderManagement.Staff.Queries.GetOrderDetailByStaff;

public sealed class GetOrderDetailByStaffHandler(
    OrderMapper orderMapper,
    IOrderRepo orderRepo,
    IUxmService uxmService,
    IAsymmetricCryptoService asymmetricCryptoService,
    IOptions<GalaxySettings> galaxySettings,
    IAsymmetricKeyCacheService  asymmetricKeyCacheService) : IQueryHandler<GetOrderDetailByStaffQuery, OrderDetailInfoDto>
{
    public async Task<OrderDetailInfoDto> Handle(GetOrderDetailByStaffQuery request, CancellationToken ct = default)
    {
        var targetOrder = await orderRepo.GetByOrderIdAsync(request.OrderId, ct);
        if (targetOrder is null)
            throw new NotFoundException();

        if (targetOrder.OrderUid.IsNullOrEmpty())
            throw new BadRequestException(MessageCode.System.ResourceNotFound);
        
        var uxmRequest = await CreateUxmRequest(request, targetOrder.OrderUid);
        var result = await uxmService.GetOrderDetailInfoAsync(uxmRequest);

        // Verify UXM signature
        var uxmPublicKey = asymmetricKeyCacheService.UxmPublicKey;
        var isValid =
            asymmetricCryptoService.VerifySignature(uxmPublicKey, result.Data, result.Signature);
        if (!isValid) throw new BadRequestException(MessageCode.System.InvalidVerifyCode, "Invalid signature.");
        
        // Mapping data
        var response = new OrderDetailInfoDto();
        targetOrder.Adapt<OrderDto>().Adapt(response);
        result.Data.Adapt(response);
        
        return response;
    }
    
    private Task<SecureRequest<GetUxmOrderDetailInfoRequest>> CreateUxmRequest(GetOrderDetailByStaffQuery request, string orderUid)
    {
        var galaxyPrivateKey = asymmetricKeyCacheService.GalaxyPrivateKey;
        var merchantNumber = galaxySettings.Value.MerchantNumber;
        
        var requestData = orderMapper.ToGetOrderDetailInfoReqData(
            query: request, 
            merchantNumber: merchantNumber, 
            tradeNo: orderUid, 
            timestamp: DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        var uxmRequest = new SecureRequest<GetUxmOrderDetailInfoRequest>
        {
            Data = requestData,
            Signature = asymmetricCryptoService.Sign(galaxyPrivateKey, requestData)
        };
        
        return Task.FromResult(uxmRequest);
    }
}