namespace game_x.api.Controllers.BackOffice.Mission;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/back-office/rewards")]
public sealed class AppSettingController : BaseApiController
{

}