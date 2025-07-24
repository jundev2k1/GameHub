using game_x.application.Features.AccountManagement.Admin.Commands.UpdateStaffStatusByAdmin;
using game_x.application.Features.AccountManagement.Admin.Queries.GetStaffDetailByAdmin;
using game_x.application.Features.AccountManagement.Admin.Commands.CreateStaff;
using game_x.application.Features.AccountManagement.Admin.Commands.SoftDeleteStaff;
using game_x.application.Features.AccountManagement.Admin.Queries.GetStaffCriteriaByAdmin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Admin.Staff;

[Authorize(Roles = AppRoles.Admin)]
[Route("api/admin/staffs")]
public class StaffController : BaseApiController
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
        var query = new GetStaffCriteriaByAdminQuery(
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("{staffId}")]
    public async Task<IActionResult> GetStaffDetailAsync(string staffId)
    {
        var result = await Mediator.Send(new GetStaffDetailByAdminQuery(staffId));
        return ApiResponseFactory.Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStaffAsync(CreateStaffCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.Created();
    }

    [HttpPatch("{staffId}/status")]
    public async Task<IActionResult> UpdateStaffStatusAsync(string staffId, UpdateStaffStatusByAdminCommand byAdminCommand)
    {
        byAdminCommand.StaffId = staffId;
        await Mediator.Send(byAdminCommand);
        return ApiResponseFactory.NoContent(code: MessageCode.System.Updated);
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> SoftDeleteStaffAsync(string userId)
    {
        var command = new SoftDeleteStaffCommand { UserId = userId };
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent(code: MessageCode.System.Deleted);
    }
}