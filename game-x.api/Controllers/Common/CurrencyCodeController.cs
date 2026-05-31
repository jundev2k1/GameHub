using game_x.application.Features.FiatCurrencies.Queries.GetFiatCurrencies;

namespace game_x.api.Controllers.Common;

[Authorize(Roles = $"{AppRoles.Root}, {AppRoles.Admin},{AppRoles.Cs},{AppRoles.User}")]
[Route("api/common")]
public sealed class CurrencyCodeController : BaseApiController
{
    [HttpGet("currency-codes")]
    public async Task<IActionResult> GetCurrencyCodeAsync()
    {
        var result = await Mediator.Send(new GetFiatCurrenciesQuery());
        return ApiResponseFactory.Ok(result);
    }
}
