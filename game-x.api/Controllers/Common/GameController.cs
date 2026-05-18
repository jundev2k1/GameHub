using game_x.api.Dtos;
using game_x.application.Features.Games.Admin.Queries.GetCurrentGameRecommends;
using game_x.application.Features.Games.Client.Queries.GetGames;
using game_x.application.Features.Games.Common.Queries.GetActiveCategories;
using game_x.application.Features.Games.Common.Queries.GetActivePlatforms;
using game_x.application.Features.Games.Common.Queries.GetActiveTypes;
using System.Reflection;

namespace game_x.api.Controllers.Common;

[Route("/api/game")]
public sealed class GameController : BaseApiController
{
    [HttpGet("list")]
    public async Task<IActionResult> GetGameListAsync([AsParameters] GetGamesRequest request)
    {
        var query = new GetGamesQuery(
            request.Keyword,
            request.Platform,
            request.Categories,
            request.GameTypes,
            request.GameTags,
            request.PageNumber ?? 1,
            request.PageSize ?? 20);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("recommendations")]
    public async Task<IActionResult> GetGameRecommendationsAsync(CancellationToken ct)
    {
        var query = new GetCurrentGameRecommendsQuery();
        var response = await Mediator.Send(query, ct);
        return ApiResponseFactory.Ok(response);
    }

    [HttpGet("platforms")]
    public async Task<IActionResult> GetPlatformListAsync(CancellationToken ct = default)
    {
        var query = new GetActivePlatformsQuery();
        var result = await Mediator.Send(query, ct);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategoryListAsync(CancellationToken ct = default)
    {
        var query = new GetActiveCategoriesQuery();
        var result = await Mediator.Send(query, ct);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("types")]
    public async Task<IActionResult> GetGameTypeListAsync(CancellationToken ct = default)
    {
        var query = new GetActiveTypesQuery();
        var result = await Mediator.Send(query, ct);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("tags/icons")]
    public async Task<IActionResult> GetGameTagIconListAsync()
    {
        var result = typeof(GameTagIcons).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(f => f.GetValue(null).ToStringOrEmpty())
            .ToArray();
        return await Task.FromResult(ApiResponseFactory.Ok(result));
    }

    [HttpGet("tags/colors")]
    public async Task<IActionResult> GetGameTagColorListAsync()
    {
        var result = typeof(GameTagColors).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(f => f.GetValue(null).ToStringOrEmpty())
            .ToArray();
        return await Task.FromResult(ApiResponseFactory.Ok(result));
    }
}