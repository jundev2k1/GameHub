namespace game_x.api.Controllers.BackOffice.Mission;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/back-office/inventory-items")]
public sealed class InventoryItemDefinitionController : BaseApiController
{

}