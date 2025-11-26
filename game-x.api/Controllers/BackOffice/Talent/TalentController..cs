using game_x.application.Features.Accounts.Admin.Commands.CreateTalent;

namespace game_x.api.Controllers.BackOffice.Talent;

[Route("api/back-office/talents")]
public sealed class TalentController : BaseApiController
{
    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateTalentAsync(CreateTalentCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.Created();
    }
}
