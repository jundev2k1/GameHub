using game_x.api.Controllers;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.share.ExternalApi.Base;
using System.Text.Json;

namespace game_x.api.Hooks;

[Route("api/webhooks/fastpay")]
public sealed class FastPayHookController(IAppLogger<FastPayHookController> logger) : BaseApiController
{
    [HttpPost("deposit-success")]
    public async Task<IActionResult> DepositSuccessAsync([FromBody] JsonElement rawJson)
    {
        logger.LogInformation("===== FastPay web hook: Deposit Sucess =====");

        string jsonString = rawJson.GetRawText();
        logger.LogInformation(jsonString);

        await Task.CompletedTask;
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
