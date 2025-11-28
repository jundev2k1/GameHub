using game_x.api.Dtos;
using game_x.application.Common.Filters;
using game_x.application.Features.SystemWallets.Queries.GetSystemWallets;
using game_x.application.Features.TalentWallets.Queries.GetTalentWalletTransactions;

namespace game_x.api.Controllers.BackOffice.SystemWallets;

[Authorize(Roles = AppRoles.Admin)]
[Route("api/back-office/system-wallets")]
public sealed class SystemWalletsController : BaseApiController
{
    [HttpGet("transactions")]
    public async Task<IActionResult> GetSystemTransactionsAsync(GetSystemTransactionsByCriteriaRequest parameters)
    {
        var paramExtends = new Dictionary<string, string>();
        if (parameters.Type.IsNotNullOrEmpty())
            paramExtends.Add("type", parameters.Type!);

        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword, paramExtends);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetTalentWalletTransactionsQuery(
            filters,
            sorts,
            parameters.PageNumber ?? 1,
            parameters.PageSize ?? 20);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetSystemWalletsAsync()
    {
        var result = await Mediator.Send(new GetSystemWalletsQuery());
        return ApiResponseFactory.Ok(result);
    }
}
