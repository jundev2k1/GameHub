using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.Games.Admin.Commands.CreateGameRecommend;
using game_x.application.Features.Games.Admin.Commands.DeleteGameRecommend;
using game_x.application.Features.Games.Admin.Commands.UpdateGameRecommend;
using game_x.application.Features.Games.Admin.Queries.GetGameRecommendDetail;
using game_x.application.Features.Games.Admin.Queries.GetRecommendsByCriteria;

namespace game_x.api.Controllers.BackOffice.Game;

[Route("api/back-office/game-recommends")]
public sealed class GameRecommendController : BaseApiController
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet]
    public async Task<IActionResult> GetGameRecommendListAsync([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetRecommendsByCriteriaQuery(
            filters,
            sorts,
            parameters.PageNumber ?? 1,
            parameters.PageSize ?? 20);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGameRecommendDetailAsync(Guid id)
    {
        var query = new GetGameRecommendDetailQuery(id);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateGameRecommendAsync(CreateGameRecommendCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent(code: MessageCode.System.Created);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateGameRecommendAsync(Guid id, UpdateGameRecommendCommand command)
    {
        await Mediator.Send(command with { Id = id });
        return ApiResponseFactory.NoContent(code: MessageCode.System.Updated);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteGameRecommendAsync(Guid id)
    {
        await Mediator.Send(new DeleteGameRecommendCommand(id));
        return ApiResponseFactory.NoContent(code: MessageCode.System.Deleted);
    }
}