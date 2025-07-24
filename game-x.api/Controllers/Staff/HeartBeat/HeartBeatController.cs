using game_x.application.Features.HeartBeat.Staff.Commands.StaffCounterHeartBeat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Staff.HeartBeat;

[Authorize(Roles = AppRoles.Staff)]
[Route("api/staff")]
public class HeartBeatController : BaseApiController
{
    [HttpPost("counters/heartbeats")]
    public async Task<IActionResult> StaffCounterHeartBeat(StaffCounterHeartBeatCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent(MessageCode.System.RefreshSuccess);
    }
}
