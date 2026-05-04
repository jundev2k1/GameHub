using game_x.api.Controllers;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Features.Transactions.Webhooks.FastPay.Commands.FastPayDepositSuccess;
using game_x.share.ExternalApi.Base;
using game_x.share.ExternalApi.FastPay.Dtos.Webhooks.DepositSuccess;
using System.Text.Json;

namespace game_x.api.Hooks;

[Route("api/webhooks/fastpay")]
public sealed class FastPayHookController(IAppLogger<FastPayHookController> logger) : BaseApiController
{
    [HttpPost("deposit-success")]
    public async Task<IActionResult> DepositSuccessAsync([FromBody] SecureRequest<DepositSucessCallbackRequest> request, CancellationToken ct = default)
    {
        logger.LogInformation("===== FastPay web hook: Deposit Sucess =====");

        var command = new FastPayDepositSuccessCommand(request.Data, request.Signature);
        await Mediator.Send(command, ct);
        return ApiResponseFactory.NoContent();
    }

    [HttpPost("deposit-failed")]
    public async Task<IActionResult> DepositFailedAsync([FromBody] JsonElement rawJson)
    {
        logger.LogInformation("===== FastPay web hook: Deposit Failed =====");

        string jsonString = rawJson.GetRawText();
        logger.LogInformation(jsonString);

        await Task.CompletedTask;
        return ApiResponseFactory.NoContent();
    }
}
