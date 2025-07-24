using game_x.application.Features.CounterManagement.Admin.Commands.CreateCounter;
using game_x.application.Features.CounterManagement.Admin.Commands.SoftDeleteCounter;
using game_x.application.Features.CounterManagement.Admin.Commands.UpdateCounter;
using game_x.application.Features.CounterManagement.Admin.Commands.UpdateCounterStatus;
using game_x.application.Features.CounterManagement.Admin.Queries.GetCounterDetail;
using game_x.application.Features.CounterManagement.Admin.Queries.GetCounterCriteriaByAdmin;
using game_x.application.Features.CounterManagement.Admin.Queries.GetCounterDetailStatistics;
using game_x.application.Features.CounterManagement.Admin.Queries.GetCounterStatistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Admin.Counter;

[Authorize(Roles = AppRoles.Admin)]
[Route("api/admin/counters")]
public class CounterController : BaseApiController
{
    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet]
    public async Task<IActionResult> GetByCriteria([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetCounterCriteriaByAdminQuery(
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetCounterStatisticsAsync([AsParameters] StatisticCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters);
        var result = await Mediator.Send(new GetCounterStatisticsQuery(filters));
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("{counterId:guid}/statistics")]
    public async Task<IActionResult> GetCounterStatisticDetailAsync(Guid counterId, [AsParameters] StatisticCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters);
        var result = await Mediator.Send(new GetCounterStatisticDetailQuery(counterId, filters));
        return ApiResponseFactory.Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCounterAsync(CreateCounterCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.Created();
    }

    [HttpGet("{counterId:guid}")]
    public async Task<IActionResult> GetByCounterId(Guid counterId)
    {
        var query = new GetCounterDetailQuery(counterId);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpDelete("{counterId:guid}")]
    public async Task<IActionResult> DeleteCounterAsync(Guid counterId)
    {
        var command = new SoftDeleteCounterCommand(counterId);
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent(MessageCode.System.Deleted);
    }

    [HttpPut("{counterId:guid}")]
    public async Task<IActionResult> UpdateCounterAsync(Guid counterId, UpdateCounterCommand request)
    {
        var command = request with { CounterId = counterId };
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent(MessageCode.System.Updated);
    }

    [HttpPatch("{counterId:guid}/status")]
    public async Task<IActionResult> UpdateCounterStatusAsync(Guid counterId, UpdateCounterStatusCommand request)
    {
        var command = request with { CounterId = counterId };
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent(MessageCode.System.Updated);
    }
}
