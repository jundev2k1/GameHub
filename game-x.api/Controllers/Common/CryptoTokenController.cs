using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.ChainTransactions.Client.Queries.GetUsdtLedgerCriteriaByUser;
using game_x.application.Features.ChainTransactions.Client.Queries.GetUsdtLedgerDetail;
using game_x.application.Features.ChainTransactions.Shared.Queries.GetCryptoTokenList;

namespace game_x.api.Controllers.Common;

[Authorize(Roles = AppRoles.User)]
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