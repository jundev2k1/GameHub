using game_x.api.Dtos;
using game_x.application.Common.Filters;
using game_x.application.Features.Games.Admin.Commands.UpdateGame;
using game_x.application.Features.Games.Admin.Queries.GetGameDetail;
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
        if (parameters.Tags.IsNotNullOrEmpty())
            paramExtends.Add("tags", parameters.Tags!);

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
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGameDetailAsync(Guid id)
    {
        var query = new GetGameDetailQuery(id);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{gameId}")]
    public async Task<IActionResult> UpdateGameAsync(Guid gameId, UpdateGameCommand command)
    {
        await Mediator.Send(command with { Id = gameId });
        return ApiResponseFactory.NoContent(code: MessageCode.System.Updated);
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactionByCriteriaAsync([AsParameters] GetGameTransactionsRequest parameters)
    {
        var paramExtends = new Dictionary<string, string>();
        if (parameters.Statuses.IsNotNullOrEmpty())
            paramExtends.Add("statuses", parameters.Statuses);
        if (parameters.Platforms.IsNotNullOrEmpty())
            paramExtends.Add("platforms", parameters.Platforms);

        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword, paramExtends);
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
