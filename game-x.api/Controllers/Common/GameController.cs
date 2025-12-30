using game_x.api.Dtos;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Features.Games.Admin.Queries.GetCurrentGameRecommends;
using game_x.application.Features.Games.Client.Queries.GetGames;
using System.Reflection;

namespace game_x.api.Controllers.Common;

[Route("/api/game")]
public sealed class GameController(
    IGameProviderCacheService gameProviderCache) : BaseApiController
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
    public async Task<IActionResult> GetGameRecommendationsAsync()
    {
        var query = new GetCurrentGameRecommendsQuery();
        var response = await Mediator.Send(query);
        return ApiResponseFactory.Ok(response);
    }

    [HttpGet("platforms")]
    public async Task<IActionResult> GetPlatformListAsync()
    {
        var result = gameProviderCache.PlatformList
            .OrderByDescending(platform => platform.Priority)
            .Select(platform => new
            {
                platform.Id,
                platform.Name,
                platform.Description
            })
            .ToArray();
        return await Task.FromResult(ApiResponseFactory.Ok(result));
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategoryListAsync()
    {
        var result = gameProviderCache.CategoryList
            .OrderByDescending(cate => cate.Priority)
            .Select(cate => new
            {
                cate.Id,
                cate.Name,
                cate.Description
            })
            .ToArray();
        return await Task.FromResult(ApiResponseFactory.Ok(result));
    }

    [HttpGet("types")]
    public async Task<IActionResult> GetGameTypeListAsync()
    {
        var result = gameProviderCache.GameTypeList
            .OrderByDescending(type => type.Priority)
            .Select(type => new
            {
                type.Id,
                type.Name,
                type.Description
            })
            .ToArray();
        return await Task.FromResult(ApiResponseFactory.Ok(result));
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