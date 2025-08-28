using game_x.application.Features.Accounts.Admin.Commands.CreateCustomerSupport;

namespace game_x.api.Controllers.Admin.Cs;

[Route("api/back-office/customer-supports")]
public sealed class CustomerSupportController : BaseApiController
{
    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateCsAsync(CreateCustomerSupportCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.Created();
    }
}
