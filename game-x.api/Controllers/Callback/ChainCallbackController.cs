using game_x.application.Features.Transactions.Shared.Commands.Callback.CryptoTransactionCallback;
using game_x.application.Features.Transactions.Shared.Commands.Callback.PaymentGatewayCallback;
using PaymentGatewayCallbackRequest = game_x.share.ExternalApi.PaymentGateway.Dtos.PaymentGatewayCallbackRequest;
using game_x.share.ExternalApi.Base;
using game_x.share.ExternalApi.Uxm.Dtos.Webhooks.CryptoCallback;

namespace game_x.api.Controllers.Callback;

[Route("api/callback")]
public class ChainTransactionCallbackController : BaseApiController
{
    [HttpPost("crypto-transaction")]
    public async Task<IActionResult> CryptoCallback(SecureRequest<CryptoCallbackRequest> request)
    {
        var command = new CryptoTransactionCallbackCommand(
            Data: request.Data,
            Signature: request.Signature);
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("payment-gateway")]
    public async Task<IActionResult> PaymentGatewayCallback(SecureRequest<PaymentGatewayCallbackRequest> request)
    {
        var command = new PaymentGatewayCallbackCommand(
            Data: request.Data,
            Signature: request.Signature);
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }
}