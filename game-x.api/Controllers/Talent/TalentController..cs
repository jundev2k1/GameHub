using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.TalentWallets.Queries.GetTalentWallets;

namespace game_x.api.Controllers.Talent;

[Authorize(Roles = AppRoles.Talent)]
[Route("api/user/talents")]
public sealed class TalentController : BaseApiController
{
    [Authorize(Roles = AppRoles.Talent)]
    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactionAsync(SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetTalentWalletsQuery(
            filters,
            sorts,
            parameters.PageNumber ?? 1,
            parameters.PageSize ?? 20);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
}
