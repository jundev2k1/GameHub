using game_x.share.ExternalApi.Uxm.Dtos;
using game_x.application.Features.ChainTransactions.Shared.Commands.Callback.CryptoTransactionCallback;

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
}
