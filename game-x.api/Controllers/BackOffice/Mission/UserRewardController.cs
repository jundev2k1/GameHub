namespace game_x.api.Controllers.BackOffice.Mission;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/back-office/user-rewards")]
public sealed class UserRewardController : BaseApiController
{

}