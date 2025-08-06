using game_x.application.Features.ChainTransactions.Admin.Commands.AdminReviewWithdrawalOrder;

namespace game_x.api.Controllers.Admin.Chain;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/admin")]
public sealed class ChainController : BaseApiController
{
    [HttpPost("tron/usdt/withdrawal/{orderId}/review")]
    public async Task<IActionResult> ReviewWithdrawalOrderAsync(Guid orderId, AdminReviewWithdrawalOrderCommand command, CancellationToken ct)
    {
        command.OrderId = orderId;
        var result = await Mediator.Send(command, ct);
        return ApiResponseFactory.Ok(result);
    }
}
