using game_x.api.Common;
using game_x.api.Dtos;
using game_x.application.Common.Filters;
using game_x.application.Features.LiveStreams.Commands.AssignTalent;
using game_x.application.Features.LiveStreams.Commands.CancelSchedule;
using game_x.application.Features.LiveStreams.Commands.CreateSchedule;
using game_x.application.Features.LiveStreams.Commands.DeleteSchedule;
using game_x.application.Features.LiveStreams.Commands.UpdateSchedule;
using game_x.application.Features.LiveStreams.Queries.GetScheduleDetail;
using game_x.application.Features.LiveStreams.Queries.GetSchedulesByCriteria;

namespace game_x.api.Controllers.BackOffice.LiveStream;

[Route("/api/back-office/livestreams")]
public sealed class LiveStreamController : BaseApiController
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet]
    public async Task<IActionResult> GetSchedulesByCriteriaAsync([AsParameters] GetLiveStreamsByCriteriaRequest parameters)
    {
        var paramExtends = new Dictionary<string, string>();
        if (parameters.Statuses.IsNotNullOrEmpty())
            paramExtends.Add("statuses", parameters.Statuses!);

        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword, paramExtends);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetSchedulesByCriteriaQuery(
            filters,
            sorts,
            parameters.PageNumber ?? 1,
            parameters.PageSize ?? 20);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetScheduleDetailAsync(Guid id)
    {
        var query = new GetScheduleDetailQuery(id);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateScheduleAsync(CreateScheduleCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPatch("{id:guid}/talent")]
    public async Task<IActionResult> AssignTalentToScheduleAsync(Guid id, AssignTalentCommand command)
    {
        await Mediator.Send(command with { Id = id });
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateScheduleAsync(Guid id, UpdateScheduleCommand command)
    {
        await Mediator.Send(command with { Id = id });
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPatch("{id:guid}/cancelation")]
    public async Task<IActionResult> CancelScheduleAsync(Guid id, CancelScheduleCommand command)
    {
        await Mediator.Send(command with { Id = id });
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteScheduleAsync(Guid id)
    {
        var command = new DeleteScheduleCommand(id);
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }
}
