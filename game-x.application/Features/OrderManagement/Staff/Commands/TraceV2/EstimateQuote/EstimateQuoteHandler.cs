using game_x.application.Contract.Infrastructure.ExternalApi.Uxm;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Features.OrderManagement.Dtos;
using game_x.application.Mappers.Order;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.share.ExternalApi.Uxm.Dtos;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.application.Features.OrderManagement.Staff.Commands.TraceV2.EstimateQuote;

public sealed class EstimateQuoteHandler(
    OrderMapper orderMapper,
    IUxmService uxmService,
    IAsymmetricCryptoService asymmetricCryptoService,
    IOptions<GalaxySettings> galaxySettings,
    IAsymmetricKeyCacheService  asymmetricKeyCacheService) : ICommandHandler<EstimateQuoteCommand, EstimateQuoteResponseDto>
{
    public async Task<EstimateQuoteResponseDto> Handle(EstimateQuoteCommand request, CancellationToken ct = default)
    {
        var uxmRequest = await CreateUxmRequest(request);
        var result = await uxmService.GetEstimateQuoteAsync(uxmRequest);

        // Verify UXM signature
        var uxmPublicKey = asymmetricKeyCacheService.UxmPublicKey;
        var isValid =
            asymmetricCryptoService.VerifySignature(uxmPublicKey, result.Data, result.Signature);
        if (!isValid) throw new BadRequestException(MessageCode.System.InvalidVerifyCode, "Invalid signature.");
         
        return result.Data.Adapt<EstimateQuoteResponseDto>();
    }
    
    private Task<SecureRequest<EstimateQuoteRequest>> CreateUxmRequest(EstimateQuoteCommand request)
    {
        var galaxyPrivateKey = asymmetricKeyCacheService.GalaxyPrivateKey;
        var merchantNumber = galaxySettings.Value.MerchantNumber;
        
        var requestData = orderMapper.ToGetEstimateQuoteReqData(
            command: request,
            merchantNumber: merchantNumber, 
            timestamp: DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        var uxmRequest = new SecureRequest<EstimateQuoteRequest>
        {
            Data = requestData,
            Signature = asymmetricCryptoService.Sign(galaxyPrivateKey, requestData)
        };
        
        return Task.FromResult(uxmRequest);
    }
}
