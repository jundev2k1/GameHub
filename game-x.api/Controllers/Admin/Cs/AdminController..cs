using game_x.application.Features.Accounts.Admin.Commands.CreateCustomerSupport;

namespace game_x.api.Controllers.Admin.Cs;

[Route("api/admin/customer-supports")]
[Authorize(Roles = AppRoles.Admin)]
public sealed class CustomerSupportController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateCsAsync(CreateCustomerSupportCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.Created();
    }
}
