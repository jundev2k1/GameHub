using game_x.application.Features.Accounts.Admin.Queries.GetSelfUserProfile;
using game_x.application.Features.Auth.Admin.Commands.ChangePasswordAdmin;

namespace game_x.api.Controllers.Admin.Me;

[Route("api/back-office/me")]
public sealed class AdminController : BaseApiController
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet]
    public async Task<IActionResult> GetUserProfileAsync()
    {
        var result = await Mediator.Send(new GetSelfUserProfileQuery());
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpPatch("password")]
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordAdminCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }
}
