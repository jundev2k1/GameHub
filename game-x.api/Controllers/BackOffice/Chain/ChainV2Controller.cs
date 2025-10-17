using game_x.application.Features.ChainTransactions.Admin.Commands.TraceV2.AdminReviewWithdrawalOrder;

namespace game_x.api.Controllers.BackOffice.Chain;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/back-office/v2/chain-transactions")]
public sealed class ChainV2Controller : BaseApiController
{
    [HttpPost("{orderId:guid}/withdrawal-review")]
    public async Task<IActionResult> ReviewWithdrawalOrderAsync(Guid orderId, [FromBody] AdminReviewWithdrawalOrderCommand command, CancellationToken ct)
    {
        var result = await Mediator.Send(command with {OrderId = orderId}, ct);
        return ApiResponseFactory.Ok(result);
    }
}
