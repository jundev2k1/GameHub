using game_x.api.Controllers;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.share.ExternalApi.Base;
using System.Text.Json;

namespace game_x.api.Hooks;

[Route("/hooks/fastpay")]
public sealed class FastPayHookController(IAppLogger<FastPayHookController> logger) : BaseApiController
{
    [HttpPost("deposit-success")]
    public async Task<IActionResult> DepositSuccessAsync(SecureRequest<string> command)
    {
        logger.LogInformation("===== FastPay web hook: Deposit Sucess =====");
        logger.LogInformation(JsonSerializer.Serialize(command));

        await Task.CompletedTask;
        return ApiResponseFactory.NoContent();
    }

    [HttpPost("deposit-failed")]
    public async Task<IActionResult> DepositFailedAsync(SecureRequest<string> command)
    {
        logger.LogInformation("===== FastPay web hook: Deposit Failed =====");
        logger.LogInformation(JsonSerializer.Serialize(command));

        await Task.CompletedTask;
        return ApiResponseFactory.NoContent();
    }
}
