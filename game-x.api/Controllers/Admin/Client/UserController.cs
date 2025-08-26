using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.Accounts.Admin.Queries.GetAllActiveTokensByAdmin;
using game_x.application.Features.Accounts.Admin.Queries.GetUserCriteriaByAdmin;
using game_x.application.Features.Accounts.Admin.Queries.GetUserDetailByAdmin;
using game_x.application.Features.Accounts.User.Commands.RevokeToken;

namespace game_x.api.Controllers.Admin.Client;

[Authorize(Roles = AppRoles.Admin)]
[Route("api/admin/users")]
public sealed class UserController : BaseApiController
{
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserDetailAsync(string userId)
    {
        var result = await Mediator.Send(new GetUserDetailByAdminQuery(userId));
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserByCriteriaAsync([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetUserCriteriaByAdminQuery(
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("{userId}/tokens")]
    public async Task<IActionResult> GetActiveTokensAsync(string userId)
    {
        var query = new GetUserActiveTokensByAdminQuery(userId);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpDelete("{userId}/tokens/{tokenId}")]
    public async Task<IActionResult> RevokeTokenAsync(string userId, Guid tokenId)
    {
        var command = new RevokeTokenCommand(userId, tokenId);
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }
}
