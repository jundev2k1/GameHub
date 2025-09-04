using game_x.api.Common;
using game_x.api.Dtos;
using game_x.application.Common.Filters;
using game_x.application.Features.Games.Admin.Queries.GetGamesByCriteria;
using game_x.application.Features.Games.Admin.Queries.GetGameTransactionDetail;
using game_x.application.Features.Games.Admin.Queries.GetGameTransactions;

namespace game_x.api.Controllers.BackOffice.Game;

[Route("/api/back-office/games")]
public sealed class GameController : BaseApiController
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet]
    public async Task<IActionResult> GetGameListAsync([AsParameters] GetGamesByCriteriaRequest parameters)
    {
        var paramExtends = new Dictionary<string, string>();
        if (parameters.Types.IsNotNullOrEmpty())
            paramExtends.Add("types", parameters.Types!);
        if (parameters.Categories.IsNotNullOrEmpty())
            paramExtends.Add("categories", parameters.Categories!);

        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword, paramExtends);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetGamesByCriteriaQuery(
            filters,
            sorts,
            parameters.PageNumber ?? 1,
            parameters.PageSize ?? 20);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactionByCriteriaAsync([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetGameTransactionsQuery(
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet("transactions/{transactionId}")]
    public async Task<IActionResult> GetTransactionByIdAsync(Guid transactionId)
    {
        var query = new GetGameTransactionDetailQuery(transactionId);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
}
