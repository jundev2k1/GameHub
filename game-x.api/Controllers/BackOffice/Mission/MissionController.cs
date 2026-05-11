namespace game_x.api.Controllers.BackOffice.Mission;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/back-office/missions")]
public sealed class MissionController : BaseApiController
{
}