using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.Accounts.Admin.Commands.CreateTalent;
using game_x.application.Features.Accounts.Admin.Queries.GetTalentCriteriaByAdmin;
using game_x.application.Features.TalentWallets.Queries.GetTalentWalletTransactions;

namespace game_x.api.Controllers.BackOffice.Talent;

[Route("api/back-office/talents")]
public sealed class TalentController : BaseApiController
{
    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet]
    public async Task<IActionResult> GetTalentsByCriteriaAsync([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetTalentCriteriaByAdminQuery(
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactionsByCriteriaAsync([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetTalentWalletTransactionsQuery(
            filters,
            sorts,
            parameters.PageNumber ?? 1,
            parameters.PageSize ?? 20);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateTalentAsync(CreateTalentCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.Created();
    }
}
