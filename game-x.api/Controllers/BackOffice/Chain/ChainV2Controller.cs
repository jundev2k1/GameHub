using game_x.application.Features.ChainTransactions.Admin.Commands.TraceV2.AdminReviewWithdrawalOrderV2;

namespace game_x.api.Controllers.BackOffice.Chain;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/back-office/v2/chain-transactions")]
public sealed class ChainV2Controller : BaseApiController
{
    [HttpPost("{orderId:guid}/withdrawal-review")]
    public async Task<IActionResult> ReviewWithdrawalOrderAsync(Guid orderId, [FromBody] AdminReviewWithdrawalOrderV2Command v2Command, CancellationToken ct)
    {
        var result = await Mediator.Send(v2Command with {OrderId = orderId}, ct);
        return ApiResponseFactory.Ok(result);
    }
}
