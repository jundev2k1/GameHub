namespace game_x.api.Controllers.BackOffice.Rewards;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/back-office/executions")]
public sealed class ExecutionController : BaseApiController
{

}