using game_x.application.Features.ChainTransactions.Client.Commands.TronUsdtWithdrawal;
using game_x.application.Features.ChainTransactions.Client.Commands.TronUsdtDeposit;

namespace game_x.api.Controllers.Client.Chain;

[Authorize(Roles = AppRoles.User)]
[Route("/api/user")]
public sealed class ChainController : BaseApiController
{
    [HttpPost("withdrawal")]
    public async Task<IActionResult> SendWithdrawVerificationCode(TronUsdtWithdrawalCommand command, CancellationToken ct)
    {
        var result = await Mediator.Send(command, ct);
        return ApiResponseFactory.Ok(result);
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> CreateDepositTransactionAsync(TronUsdtDepositCommand command, CancellationToken ct)
    {
        var result = await Mediator.Send(command, ct);
        return ApiResponseFactory.Ok(result);
    }
}
