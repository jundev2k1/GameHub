using game_x.application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Common;

[Authorize(Roles = $"{AppRoles.Root}, {AppRoles.Admin},{AppRoles.Staff},{AppRoles.User}")]
[Route("api/common")]
public class CurrencyCodeController : BaseApiController
{
    [HttpGet("currency-codes")]
    public IActionResult GetCurrencyCode()
    {
        return ApiResponseFactory.Ok(CurrencyCodeProvider.All());
    }
}
