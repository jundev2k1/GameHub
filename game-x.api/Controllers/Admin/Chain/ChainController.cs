using game_x.application.Features.ChainTransactions.Admin.Commands.AdminReviewWithdrawalOrder;

namespace game_x.api.Controllers.Admin.Chain;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/admin")]
public sealed class ChainController : BaseApiController
{
    [HttpPost("withdrawal/{orderId}/review")]
    public async Task<IActionResult> ReviewWithdrawalOrderAsync(Guid orderId, [FromBody] AdminReviewWithdrawalOrderCommand command, CancellationToken ct)
    {
        var result = await Mediator.Send(command with {OrderId = orderId}, ct);
        return ApiResponseFactory.Ok(result);
    }
}
