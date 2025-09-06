using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.Games.Admin.Commands.CreateGameTag;
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

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateGameTypeAsync(CreateGameTagCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }
}
