using game_x.application.Features.OrderManagement.Client.Commands.Trade.Buy;
using game_x.application.Features.OrderManagement.Client.Commands.Trade.Sell;

namespace game_x.api.Controllers.Staff.Order;

[Route("v2/order/tron/usdt")]
public class OrderV2Controller : BaseApiController
{

    [HttpPost("deposit")]
    public async Task<IActionResult> CreateDepositOrderAsync(CreateBuyOrderCommand command)
    {
        return ApiResponseFactory.Ok(await Mediator.Send(command), MessageCode.System.Accepted);
    }
}
[Route("v1/order/tron/usdt")]
public class OrderV1Controller : BaseApiController
{
    [HttpPost("withdrawal")]
    public async Task<IActionResult> CreateWithdrawalOrderAsync(CreateSellOrderCommand command)
    {
        return ApiResponseFactory.Ok(await Mediator.Send(command), MessageCode.System.Accepted);
    }
}