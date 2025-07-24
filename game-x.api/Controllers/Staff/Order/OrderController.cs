using game_x.application.Features.OrderManagement.Staff.Commands.ReviewOrderByStaff;
using game_x.application.Features.OrderManagement.Staff.Commands.Trade.Buy;
using game_x.application.Features.OrderManagement.Staff.Commands.Trade.Sell;
using game_x.application.Features.OrderManagement.Staff.Commands.TraceV2.EstimateQuote;
using game_x.application.Features.OrderManagement.Staff.Queries.GetOrderDetailByStaff;
using game_x.application.Features.OrderManagement.Staff.Queries.GetOrderCriteriaForUserByStaff;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Staff.Order;

[Authorize(Roles = AppRoles.Staff)]
[Route("api/staff")]
public class OrderController : BaseApiController
{
    [HttpGet("users/{userId}/orders")]
    public async Task<IActionResult> GetOrderOfUserByCriteriaAsync(
        string userId,
        [AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetOrderCriteriaForUserByStaffQuery(
            userId,
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("orders/{orderId}")]
    public async Task<IActionResult> GetByOrderIdAsync(Guid orderId)
    {
        var query = new GetOrderDetailByStaffQuery(orderId);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpPost("orders/buy")]
    public async Task<IActionResult> CreateBuyOrderAsync(CreateBuyOrderCommand command)
    {
        return ApiResponseFactory.Ok(await Mediator.Send(command), MessageCode.Order.OrderCreated);
    }

    [HttpPost("orders/sell")]
    public async Task<IActionResult> CreateSellOrderAsync(CreateSellOrderCommand command)
    {
        return ApiResponseFactory.Ok(await Mediator.Send(command), MessageCode.Order.OrderCreated);
    }

    [HttpPatch("orders/{orderId}/review-decision")]
    public async Task<IActionResult> ReviewOrderAsync(Guid orderId, ReviewOrderByStaffCommand command)
    {
        await Mediator.Send(command with { OrderId = orderId });
        return ApiResponseFactory.NoContent(MessageCode.Order.OrderStatusUpdated);
    }
    
    [HttpPost("orders/estimate-quote")]
    public async Task<IActionResult> GetEstimateQuoteAsync(EstimateQuoteCommand command)
    {
        return ApiResponseFactory.Ok(await Mediator.Send(command));
    }
}