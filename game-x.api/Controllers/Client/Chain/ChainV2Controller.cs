using game_x.application.Features.ChainTransactions.Client.Commands.TraceV2.TronUsdtWithdrawalV2;
using game_x.application.Features.ChainTransactions.Client.Commands.TraceV2.TronUsdtDepositV2;
using game_x.application.Features.ChainTransactions.Client.Queries.GetMyTransactionDetail;

namespace game_x.api.Controllers.Client.Chain;

[Authorize(Roles = AppRoles.User)]
[Route("/api/user/v2/chain-transactions")]
public sealed class ChainV2Controller : BaseApiController
{
    [HttpPost("withdrawal")]
    public async Task<IActionResult> CreateWithdrawalTransactionAsync(TronUsdtWithdrawalV2Command v2Command, CancellationToken ct)
    {
        var result = await Mediator.Send(v2Command, ct);
        return ApiResponseFactory.Ok(result);
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> CreateDepositTransactionAsync(TronUsdtDepositV2Command v2Command, CancellationToken ct)
    {
        var result = await Mediator.Send(v2Command, ct);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpGet("{transactionId:guid}")]
    public async Task<IActionResult> GetTransactionByIdAsync(Guid transactionId)
    {
        var query = new GetMyTransactionDetailQuery(transactionId);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
}
