using game_x.application.Exceptions;
using game_x.application.Features.OrderManagement.Admin.Commands.ReviewOrderByAdmin;
using game_x.application.Features.OrderManagement.Admin.Commands.UpdateOrderInfoByAdmin;
using game_x.application.Features.OrderManagement.Admin.Queries.GetOrderCriteriaByAdmin;
using game_x.application.Features.OrderManagement.Admin.Queries.GetOrderDetailByAdmin;
using game_x.application.Features.OrderManagement.Admin.Queries.GetOrderStatistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Admin.Order;

[Authorize(Roles = AppRoles.Admin)]
[Route("api/admin/orders")]
public class OrderController : BaseApiController
{
    [HttpGet("statistics")]
    public async Task<IActionResult> GetOrderStatistics()
    {
        var result = await Mediator.Send(new GetOrderStatisticsQuery());
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetByCriteria([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetOrderCriteriaByAdminQuery(
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetByOrderId(Guid orderId)
    {
        var query = new GetOrderDetailByAdminQuery(orderId);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpPut("{orderId}")]
    public async Task<IActionResult> UpdateOrder(Guid orderId, UpdateOrderInfoByAdminCommand infoByAdminCommand)
    {
        if (orderId != infoByAdminCommand.OrderId)
            throw new InvalidArgumentException("Order ID mismatch.");

        await Mediator.Send(infoByAdminCommand);
        return ApiResponseFactory.NoContent(code: MessageCode.Order.OrderUpdated);
    }

    [HttpPatch("{orderId}/review-decision")]
    public async Task<IActionResult> ReviewOrderAsync(Guid orderId, ReviewOrderByAdminCommand command)
    {
        await Mediator.Send(command with { OrderId = orderId });
        return ApiResponseFactory.NoContent(code: MessageCode.Order.OrderStatusUpdated);
    }
}
