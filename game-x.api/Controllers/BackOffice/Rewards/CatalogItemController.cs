using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Features.Rewards.Commands.CatalogItems.Create;
using game_x.application.Features.Rewards.Commands.CatalogItems.Remove;
using game_x.application.Features.Rewards.Commands.CatalogItems.Update;

namespace game_x.api.Controllers.BackOffice.Rewards;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/back-office/catalog-items")]
public sealed class CatalogItemController(ICatalogItemCacheService service) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetListAsync(CancellationToken ct = default)
    {
        var result = await service.GetAll(ct);
        return ApiResponseFactory.Ok(result ?? []);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromForm] CreateCatalogItemCommand cmd, CancellationToken ct = default)
    {
        var result = await Mediator.Send(cmd, ct);
        return ApiResponseFactory.Created(result);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoveItemAsync(Guid id, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new CatalogItemRemoveCommand(id), ct);
        return ApiResponseFactory.Created(result);
    }
    
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromForm] UpdateCatalogItemCommand cmd, CancellationToken ct = default)
    {
        var result = await Mediator.Send(cmd with {Id = id}, ct);
        return ApiResponseFactory.Created(result);
    }
}