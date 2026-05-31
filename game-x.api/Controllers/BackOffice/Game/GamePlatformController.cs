using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Features.Games.Admin.Commands.UpdateGamePlatform;
using game_x.application.Features.Games.Admin.Commands.UpdateGamePlatformTranslations;
using game_x.application.Features.Games.Admin.Queries.GetGamePlatformDetail;

namespace game_x.api.Controllers.BackOffice.Game;

[Route("api/back-office/game-platforms")]
public sealed class GamePlatformController(
    IGameProviderCacheService gameProviderCache) : BaseApiController
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet]
    public async Task<IActionResult> GetGamePlatformListAsync()
    {
        var result = gameProviderCache.PlatformList
            .OrderByDescending(platform => platform.Priority)
            .Select(platform => new
            {
                platform.Id,
                platform.Name,
                platform.Description,
                platform.Note,
                platform.Priority,
                platform.IsActive
            })
            .ToArray();
        return await Task.FromResult(ApiResponseFactory.Ok(result));
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGameTypeAsync(Guid id)
    {
        var query = new GetGamePlatformDetailQuery(id);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateGamePlatformAsync(Guid id, UpdateGamePlatformCommand command)
    {
        await Mediator.Send(command with { Id = id });
        return ApiResponseFactory.NoContent(MessageCode.System.Updated);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost("{platformId:guid}/translations")]
    public async Task<IActionResult> UpsertGameTranslationsAsync(
        [FromRoute] Guid platformId,
        [FromBody] UpdateGamePlatformTranslationsCommand command)
    {
        await Mediator.Send(command with { GamePlatformId = platformId });
        return ApiResponseFactory.NoContent(code: MessageCode.System.Updated);
    }
}