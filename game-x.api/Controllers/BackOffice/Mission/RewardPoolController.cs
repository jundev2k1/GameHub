namespace game_x.api.Controllers.BackOffice.Mission;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/back-office/reward-pools")]
public sealed class RewardPoolController : BaseApiController
{

}