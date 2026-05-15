using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Features.Rewards.Commands.CreateCatalogItem;

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
}