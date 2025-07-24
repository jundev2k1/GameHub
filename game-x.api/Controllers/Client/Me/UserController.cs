using game_x.application.Features.Auth.Commands.ChangePassword;
using game_x.application.Features.AccountManagement.User.Commands.UserSelfUpdate;
using game_x.application.Features.AccountManagement.User.Queries.GetSelfUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Client.Me;

[Authorize(Roles = AppRoles.User)]
[Route("api/user")]
public class UserController : BaseApiController
{
    [HttpGet("me")]
    public async Task<IActionResult> GetUserDetailAsync()
    {
        var result = await Mediator.Send(new GetSelfUserQuery());
        return ApiResponseFactory.Ok(result);
    }

    [HttpPut("me/password")]
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent(code: MessageCode.User.UserChangePasswordSuccess);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateUserAsync(UserSelfUpdateCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent(code: MessageCode.System.Updated);
    }
}
