using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.UserGameSessions.Queries.GetGameSessions;

namespace game_x.api.Controllers.BackOffice.UserGameSessions;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/back-office/user-game-sessions")]
public class UserGameSessionController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetGameSessionsByCriteriaAsync([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetGameSessionsQuery(
            filters,
            sorts,
            parameters.PageNumber ?? 1,
            parameters.PageSize ?? 20);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
}
