using game_x.application.Features.Auth.Commands.ChangePassword;
using game_x.application.Features.AccountManagement.Staff.Queries.GetSelfStaff;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Staff.Me;

[Authorize(Roles = AppRoles.Staff)]
[Route("api/staff")]
public class StaffController : BaseApiController
{
    [HttpGet("me")]
    public async Task<IActionResult> GetStaffDetailAsync()
    {
        var result = await Mediator.Send(new GetSelfStaffQuery());
        return ApiResponseFactory.Ok(result);
    }

    [HttpPut("me/password")]
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent(code: MessageCode.User.UserChangePasswordSuccess);
    }
}
