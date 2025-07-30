using game_x.application.Features.OrderManagement.Client.Commands.Trade.Buy;

namespace game_x.api.Controllers.Staff.Order;

[Route("v2/order/tron/usdt")]
public class OrderController : BaseApiController
{

    [HttpPost("deposit")]
    public async Task<IActionResult> CreateBuyOrderAsync(CreateBuyOrderCommand command)
    {
        return ApiResponseFactory.Ok(await Mediator.Send(command), MessageCode.System.Accepted);
    }


}