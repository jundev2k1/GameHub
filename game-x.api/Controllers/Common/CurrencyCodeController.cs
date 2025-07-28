using game_x.application.Common;

namespace game_x.api.Controllers.Common;

[Authorize(Roles = $"{AppRoles.Root}, {AppRoles.Admin},{AppRoles.Cs},{AppRoles.User}")]
[Route("api/common")]
public sealed class CurrencyCodeController : BaseApiController
{
    [HttpGet("currency-codes")]
    public IActionResult GetCurrencyCodeAsync()
    {
        return ApiResponseFactory.Ok(CurrencyCodeProvider.All());
    }
}
