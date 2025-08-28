using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.Accounts.Admin.Queries.GetAllActiveTokensByAdmin;
using game_x.application.Features.Accounts.Admin.Queries.GetUserCriteriaByAdmin;
using game_x.application.Features.Accounts.Admin.Queries.GetUserDetailByAdmin;
using game_x.application.Features.Accounts.User.Commands.RevokeToken;

namespace game_x.api.Controllers.BackOffice.Client;

[Route("api/back-office/users")]
public sealed class UserController : BaseApiController
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserDetailAsync(string userId)
    {
        var result = await Mediator.Send(new GetUserDetailByAdminQuery(userId));
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
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

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet("{userId}/tokens")]
    public async Task<IActionResult> GetActiveTokensAsync(string userId)
    {
        var query = new GetUserActiveTokensByAdminQuery(userId);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpDelete("{userId}/tokens/{tokenId}")]
    public async Task<IActionResult> RevokeTokenAsync(string userId, Guid tokenId)
    {
        var command = new RevokeTokenCommand(userId, tokenId);
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }
}
