using game_x.application.Features.OrderManagement.Staff.Commands.TraceV2.Buy;
using game_x.application.Features.OrderManagement.Staff.Commands.TraceV2.Sell;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Staff.Order;

[Authorize(Roles = AppRoles.Staff)]
[Route("api/v2/staff")]
public class OrderV2Controller : BaseApiController
{
    [HttpPost("orders/buy")]
    public async Task<IActionResult> CreateBuyOrderAsync(CreateBuyOrderV2Command command)
    {
        return ApiResponseFactory.Ok(await Mediator.Send(command), MessageCode.Order.OrderCreated);
    }

    [HttpPost("orders/sell")]
    public async Task<IActionResult> CreateSellOrderAsync(CreateSellOrderV2Command command)
    {
        return ApiResponseFactory.Ok(await Mediator.Send(command), MessageCode.Order.OrderCreated);
    }
}
