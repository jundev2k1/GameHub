using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.LiveStreams.Categories.Commands.CreateCategory;
using game_x.application.Features.LiveStreams.Categories.Commands.DeleteCategory;
using game_x.application.Features.LiveStreams.Categories.Commands.UpdateCategory;
using game_x.application.Features.LiveStreams.Categories.Commands.UpdateCategoryStatus;
using game_x.application.Features.LiveStreams.Categories.Queries.GetCategoriesByCriteria;
using game_x.application.Features.LiveStreams.Categories.Queries.GetCategoryDetail;

namespace game_x.api.Controllers.BackOffice.LiveStream;

[Route("api/back-office/livestream-categories")]
public sealed class LiveStreamCategoryController : BaseApiController
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet]
    public async Task<IActionResult> GetCategoriesByCriteriaAsync([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetCategoriesByCriteriaQuery(
            filters,
            sorts,
            parameters.PageNumber ?? 1,
            parameters.PageSize ?? 20);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCategoryDetailAsync(Guid id)
    {
        var query = new GetCategoryDetailQuery(id);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateLiveStreamCategoryAsync(CreateCategoryCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateLiveStreamCategoryAsync(Guid id, UpdateCategoryCommand command)
    {
        await Mediator.Send(command with { Id = id });
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateLiveStreamCategoryStatusAsync(Guid id, UpdateCategoryStatusCommand command)
    {
        await Mediator.Send(command with { Id = id });
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteLiveStreamCategoryAsync(Guid id, DeleteCategoryCommand command)
    {
        await Mediator.Send(command with { Id = id });
        return ApiResponseFactory.NoContent();
    }
}
