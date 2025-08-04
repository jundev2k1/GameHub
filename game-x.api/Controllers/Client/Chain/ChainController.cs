using game_x.application.Features.ChainTransactions.Client.Commands.TronUsdtWithdrawal;

namespace game_x.api.Controllers.Client.Chain;

[Authorize(Roles = AppRoles.User)]
[Route("/api/user")]
public sealed class ChainController: BaseApiController
{
    [HttpPost("tron/usdt/withdrawal")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SendWithdrawVerificationCode(TronUsdtWithdrawalCommand command, CancellationToken ct)
    {
        var result = await Mediator.Send(command, ct);
        return ApiResponseFactory.Ok(result);
    }
}