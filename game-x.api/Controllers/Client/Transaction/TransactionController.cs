using game_x.application.Features.ChainTransactionManagement.Client.Commands.Trade.Deposit;

namespace game_x.api.Controllers.Staff.Transaction;

[Route("v2/order/tron/usdt")]
public class DepositTransactionController : BaseApiController
{

    [HttpPost("deposit")]
    public async Task<IActionResult> CreateDepositTransactionAsync(CreateDepositChainTransactionCommand command)
    {
        return ApiResponseFactory.Ok(await Mediator.Send(command), MessageCode.System.Accepted);
    }
}
