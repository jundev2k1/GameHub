using game_x.application.Features.S2s.Commands.CreateS2sClientSetting;

namespace game_x.api.Controllers.Root.S2s;

[Authorize(Roles = AppRoles.Root)]
[Route("api/root/s2s/clients")]
public sealed class S2sClientSettingController : BaseApiController
{
    /// <summary>
    /// Creates a new setting for a third-party Server-to-Server (S2S) client
    /// </summary>
    /// <remarks>
    /// This API creates and associates a configuration setting for a third-party S2S client
    /// A setting represents an integration key and configuration
    /// for a specific platform or channel, such as web, mobile app, or device
    /// <br />
    /// Each S2S client may have multiple settings, one per supported platform,
    /// enabling secure server-to-server communication across different environments
    /// </remarks>
    /// <param name="clientId">The unique identifier of the third-party S2S client to which the will be added</param>
    /// <param name="command">The request payload containing platform-specific keys and configuration details for the S2S client</param>
    /// <returns>Returns HTTP 201 (Created) when the S2S client setting is successfully created</returns>
    [HttpPost("{clientId}/settings")]
    public async Task<IActionResult> CreateSettingAsync(string clientId, CreateS2sClientSettingCommand command)
    {
        await Mediator.Send(command with { ClientId = clientId });
        return ApiResponseFactory.Created();
    }
}
