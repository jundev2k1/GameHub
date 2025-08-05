using game_x.share.ExternalApi.Uxm.Dtos;
using game_x.api.Controllers;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using game_x.application.Features.ChainTransactions.Callback;

namespace game_x.api.Controllers.Callback;

[Route("callback")]
public class ChainTransactionCallbackController : BaseApiController
{
    [HttpPost("crypto")]
    public async Task<IActionResult> CryptoCallback(SecureRequest<CryptoCallbackRequest> request)
    {
        var command = new UpdateChainTransactionCallbackCommand(
            Data: request.Data,
            Signature: request.Signature);
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}
