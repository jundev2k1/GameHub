namespace game_x.api.Controllers.BackOffice.Mission;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/back-office/executions")]
public sealed class ExecutionController : BaseApiController
{

}