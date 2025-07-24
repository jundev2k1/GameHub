using game_x.application.Features.CounterManagement.Admin.Commands.GenerateQrCodeCounter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Admin.Counter;

[Authorize(Roles = AppRoles.Admin)]
[Route("api/admin/counters")]
public class QrCodeController : BaseApiController
{
    [HttpPost("qr-codes/identify")]
    public async Task<IActionResult> GenerateQrCode(GenerateQrCodeCounterCommand command)
    {
        var counterToken = await Mediator.Send(command);
        return ApiResponseFactory.Ok(counterToken);
    }
}
