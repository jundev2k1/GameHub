using game_x.application.Features.OrderManagement.Client.Queries.GetOrderCriteriaByClient;
using game_x.application.Features.OrderManagement.Client.Queries.GetOrderDetailByClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Client.Order;

[Authorize(Roles = AppRoles.User)]
[Route("api/user/orders")]
public class OrderController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetByCriteriaAsync([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetOrderCriteriaByClientQuery(
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetByOrderIdAsync(Guid orderId)
    {
        var query = new GetOrderDetailByClientQuery(orderId);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
}
