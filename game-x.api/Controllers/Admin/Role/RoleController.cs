using game_x.application.Features.UserRoles.Admin.Queries;

namespace game_x.api.Controllers.Admin.Role;

[Route("api/back-office/roles")]
public sealed class RoleController : BaseApiController
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet]
    public async Task<IActionResult> GetAllRolesAsync()
    {
        var result = await Mediator.Send(new GetAllRoleQuery());
        return ApiResponseFactory.Ok(result);
    }
}
