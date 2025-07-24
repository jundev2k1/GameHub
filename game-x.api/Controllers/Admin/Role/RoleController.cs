using game_x.application.Features.UserRole.Admin.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Admin.Role;

[Authorize(Roles = AppRoles.Admin)]
[Route("api/admin/roles")]
public class RoleController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        var result = await Mediator.Send(new GetAllRoleQuery());
        return ApiResponseFactory.Ok(result);
    }
}
