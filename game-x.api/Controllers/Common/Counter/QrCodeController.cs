using game_x.application.Features.CounterManagement.Staff.ResolveQrCodeCounter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Common.Counter;

[Route("api/common/counters")]
public class QrCodeController : BaseApiController
{
    [AllowAnonymous]
    [HttpPost("qr-codes/identify/validate")]
    public async Task<IActionResult> ValidateQrCode(ResolveQrCodeCounterCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }
}
