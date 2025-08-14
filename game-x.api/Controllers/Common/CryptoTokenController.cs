using game_x.application.Features.ChainTransactions.Shared.Queries.GetCryptoTokenList;

namespace game_x.api.Controllers.Common;

[Authorize(Roles = $"{AppRoles.Root},{AppRoles.Admin},{AppRoles.Cs},{AppRoles.User}")]
[Route("/api/crypto-tokens")]
public class CryptoTokenController : BaseApiController
{
    [HttpGet()]
    public async Task<IActionResult> GetCryptoTokenAsync()
    {
        var result = await Mediator.Send(new GetCryptoTokenListQuery());
        return ApiResponseFactory.Ok(result);
    }
}