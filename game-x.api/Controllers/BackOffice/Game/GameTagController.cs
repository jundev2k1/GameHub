using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.Games.Admin.Commands.CreateGameTag;
using game_x.application.Features.Games.Admin.Commands.DeleteGameTag;
using game_x.application.Features.Games.Admin.Commands.UpdateGameTag;
using game_x.application.Features.Games.Admin.Queries.GetGameTagsByCriteria;

namespace game_x.api.Controllers.BackOffice.Game;

[Route("/api/back-office/game-tags")]
public sealed class GameTagController : BaseApiController
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet]
    public async Task<IActionResult> GetGameTagListAsync([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetGameTagsByCriteriaQuery(
            filters,
            sorts,
            parameters.PageNumber ?? 1,
            parameters.PageSize ?? 20);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateGameTagAsync(CreateGameTagCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.Created();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{tagId:guid}")]
    public async Task<IActionResult> UpdateGameTagAsync(Guid tagId, UpdateGameTagCommand command)
    {
        await Mediator.Send(command with { Id = tagId });
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{tagId:guid}")]
    public async Task<IActionResult> DeleteGameTagAsync(Guid tagId)
    {
        var command = new DeleteGameTagCommand(tagId);
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }
}
