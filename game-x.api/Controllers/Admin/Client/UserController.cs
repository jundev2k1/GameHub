using game_x.application.Features.AccountManagement.Admin.Commands.UpdateUserStatusByAdmin;
using game_x.application.Features.AccountManagement.Admin.Queries.GetUserDetailByAdmin;
using game_x.application.Features.AccountManagement.Admin.Queries.GetUserStatistics;
using game_x.application.Features.AccountManagement.Admin.Commands.SoftDeleteUser;
using game_x.application.Features.AccountManagement.Admin.Queries.GetUserCriteriaByAdmin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Admin.Client;

[Authorize(Roles = AppRoles.Admin)]
[Route("api/admin/users")]
public class UserController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetByCriteria([AsParameters] SearchCriteriaRequest parameters)
    {
        var @params = HttpContext.Request.Query
            .ToDictionary(q => q.Key, q => q.Value.FirstOrDefault());
        var filters = QueryConverter.ToFilters(
            filters: parameters.Filters,
            searchText: parameters.Keyword,
            @params!,
            ignoreFields: ["filter", "sort"]);

        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetUserCriteriaByAdminQuery(
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserDetailAsync(string userId)
    {
        var result = await Mediator.Send(new GetUserDetailByAdminQuery(userId));
        return ApiResponseFactory.Ok(result);
    }

    [HttpPatch("{userId}/status")]
    public async Task<IActionResult> UpdateUserStatusAsync(string userId, [FromBody] UpdateUserStatusByAdminCommand byAdminCommand)
    {
        byAdminCommand.UserId = userId;
        await Mediator.Send(byAdminCommand);
        return ApiResponseFactory.NoContent(code: MessageCode.System.Updated);
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> SoftDeleteUserAsync(string userId)
    {
        var command = new SoftDeleteUserCommand { UserId = userId };
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent(code: MessageCode.System.Deleted);
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetUserStatistics()
    {
        var result = await Mediator.Send(new GetUserStatisticsQuery());
        return ApiResponseFactory.Ok(result);
    }
}