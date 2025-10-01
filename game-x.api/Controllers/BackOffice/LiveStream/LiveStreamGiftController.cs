using game_x.api.Common;
using game_x.api.Dtos;
using game_x.application.Common.Files;
using game_x.application.Common.Filters;
using game_x.application.Features.LiveStreams.Gifts.Commands.CreateLiveStreamGift;
using game_x.application.Features.LiveStreams.Gifts.Commands.DeleteLiveStreamGift;
using game_x.application.Features.LiveStreams.Gifts.Commands.UpdateLiveStreamGift;
using game_x.application.Features.LiveStreams.Gifts.Commands.UpdateLiveStreamGiftAnimation;
using game_x.application.Features.LiveStreams.Gifts.Commands.UpdateLiveStreamGiftIcon;
using game_x.application.Features.LiveStreams.Gifts.Commands.UpdateLiveStreamGiftStatus;
using game_x.application.Features.LiveStreams.Gifts.Queries.GetLiveStreamGiftDetail;
using game_x.application.Features.LiveStreams.Gifts.Queries.GetLiveStreamGiftsByCriteria;

namespace game_x.api.Controllers.BackOffice.LiveStream;

[Route("api/back-office/livestream-gifts")]
public sealed class LiveStreamGiftController : BaseApiController
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet]
    public async Task<IActionResult> GetGiftsByCriteriaAsync([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetLiveStreamGiftsByCriteriaQuery(
            filters,
            sorts,
            parameters.PageNumber ?? 1,
            parameters.PageSize ?? 20);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGiftDetailAsync(Guid id)
    {
        var query = new GetLiveStreamGiftDetailQuery(id);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateGiftAsync(CreateLiveStreamGiftCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateGiftAsync(Guid id, UpdateLiveStreamGiftCommand command)
    {
        await Mediator.Send(command with { Id = id });
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = $"{AppRoles.Admin}")]
    [HttpPatch("{id:guid}/icon")]
    public async Task<IActionResult> UpdateGiftIconAsync(Guid id, UpdateImageRequest request)
    {
        var command = new UpdateLiveStreamGiftIconCommand(id, FileUpload.FromFormFile(request.Image));
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = $"{AppRoles.Admin}")]
    [HttpPatch("{id:guid}/animation")]
    public async Task<IActionResult> UpdateGiftAnimationAsync(Guid id, UpdateImageRequest request)
    {
        var command = new UpdateLiveStreamGiftAnimationCommand(id, FileUpload.FromFormFile(request.Image));
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = $"{AppRoles.Admin}")]
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateGiftStatusAsync(Guid id, UpdateLiveStreamGiftStatusCommand command)
    {
        var result = await Mediator.Send(command with { Id = id });
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteGiftAsync(Guid id)
    {
        var command = new DeleteLiveStreamGiftCommand(id);
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }
}
