using game_x.application.Features.AccountManagement.Staff.Commands.ValidateUserQrCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Staff.Code;

[Authorize(Roles = AppRoles.Staff)]
[Route("api/staff/qr-codes")]
public class QrCodeController : BaseApiController
{
    [HttpPost("identify/validate")]
    public async Task<IActionResult> ValidateQrCode(ValidateUserQrCodeCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result, MessageCode.System.ValidationSuccess);
    }
}
