using game_x.application.Features.Accounts.User.Commands.GenerateSelfUserQrCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Client.Code;

[Route("api/user/qr-codes")]
public sealed class QrCodeController : BaseApiController
{
    [Authorize(Roles = AppRoles.User)]
    [HttpPost("identify")]
    public async Task<IActionResult> GenerateQrCodeAsync()
    {
        var result = await Mediator.Send(new GenerateSelfUserQrCodeCommand());
        return ApiResponseFactory.Ok(result);
    }
}
