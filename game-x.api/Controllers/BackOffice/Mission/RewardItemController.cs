namespace game_x.api.Controllers.BackOffice.Mission;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/back-office/reward-items")]
public sealed class RewardItemController : BaseApiController
{

}