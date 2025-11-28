using game_x.application.Features.AppSettings.Commands.UpdateSettings;
using game_x.application.Features.AppSettings.DTOs;

namespace game_x.api.Controllers.BackOffice.AppSettings;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/back-office/app-settings")]
public sealed class AppSettingController : BaseApiController
{
    [HttpPut]
    public async Task<IActionResult> UpdateSettingsAsync(AppSettingInputDto[] request)
    {
        await Mediator.Send(new UpdateSettingsCommand(request));
        return ApiResponseFactory.NoContent();
    }
}
