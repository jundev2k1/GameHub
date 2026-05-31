using game_x.application.Features.Accounts.Root.Commands.CreateAdmin;
using game_x.application.Features.Accounts.Root.Commands.SoftDeleteAdmin;
using game_x.application.Features.Accounts.Root.Queries.GetAdminById;

namespace game_x.api.Controllers.Root.Admin;

[Route("api/root/admins")]
[Authorize(Roles = AppRoles.Root)]
public sealed class AdminController : BaseApiController
{
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
