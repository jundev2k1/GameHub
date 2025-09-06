using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.Games.Admin.Commands.CreateGameType;
using game_x.application.Features.Games.Admin.Commands.DeleteGameType;
using game_x.application.Features.Games.Admin.Commands.UpdateGameType;
using game_x.application.Features.Games.Admin.Queries.GetGameTypeDetail;
using game_x.application.Features.Games.Admin.Queries.GetTypeByCriteria;

namespace game_x.api.Controllers.BackOffice.Game;

[Route("/api/back-office/game-types")]
public sealed class GameTypeController : BaseApiController
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet]
    public async Task<IActionResult> GetGameTypeListAsync([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetTypeByCriteriaQuery(
            filters,
            sorts,
            parameters.PageNumber ?? 1,
            parameters.PageSize ?? 20);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGameTypeAsync(Guid id)
    {
        var query = new GetGameTypeDetailQuery(id);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateGameTypeAsync(CreateGameTypeCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent(MessageCode.System.Created);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateGameTypeAsync(Guid id, UpdateGameTypeCommand command)
    {
        await Mediator.Send(command with { Id = id });
        return ApiResponseFactory.NoContent(MessageCode.System.Updated);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteGameTypeAsync(Guid id)
    {
        await Mediator.Send(new DeleteGameTypeCommand(id));
        return ApiResponseFactory.NoContent(MessageCode.System.Deleted);
    }
}
