using game_x.application.Features.AccountManagement.Root.Commands.CreateAdmin;
using game_x.application.Features.AccountManagement.Root.Commands.SoftDeleteAdmin;
using game_x.application.Features.AccountManagement.Root.Queries.GetAdminByCriteria;
using game_x.application.Features.AccountManagement.Root.Queries.GetAdminById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Root;

[Route("api/root/admins")]
[Authorize(Roles = AppRoles.Root)]
public class AdminController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetByCriteriaAsync([AsParameters] SearchCriteriaRequest parameters)
    {
        var @params = HttpContext.Request.Query
            .ToDictionary(q => q.Key, q => q.Value.FirstOrDefault());
        var filters = QueryConverter.ToFilters(
            filters: parameters.Filters,
            searchText: parameters.Keyword,
            @params!,
            ignoreFields: ["filter", "sort"]);

        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetAdminByCriteriaQuery(
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> SoftGetAdminAsync(string userId)
    {
        var query = new GetAdminByIdQuery(userId);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAdminAsync(CreateAdminCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.Created();
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> SoftDeleteAdminAsync(string userId)
    {
        var command = new SoftDeleteAdminCommand(userId);
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent(MessageCode.System.Deleted);
    }
}
