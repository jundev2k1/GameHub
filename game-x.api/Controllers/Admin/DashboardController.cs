using game_x.application.Features.Dashboard.Admin.Commands.RefreshDashboard;
using game_x.application.Features.Dashboard.Admin.Queries.GetDashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Admin;

[Authorize(Roles = AppRoles.Admin)]
[Route("api/admin/dashboard")]
public sealed class DashboardController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var result = await Mediator.Send(new GetDashboardQuery());
        return ApiResponseFactory.Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshDashboard()
    {
        await Mediator.Send(new RefreshDashboardCommand());
        return ApiResponseFactory.NoContent(code: MessageCode.System.RefreshSuccess);
    }
}
