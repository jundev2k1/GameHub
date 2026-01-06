using game_x.application.Features.S2s.Commands.CreateS2sClientSetting;
using game_x.application.Features.S2s.Commands.UpdateS2sClientSetting;

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

    /// <summary>
    /// Updates a setting of a third-party Server-to-Server (S2S) client
    /// </summary>
    /// <remarks>
    /// This API updates an existing S2S client setting that represents an integration
    /// key and configuration for a specific platform or channel (such as web, mobile
    /// app, or device)
    /// <br/>
    /// The setting is identified by the provided app code and is associated with
    /// the specified third-party S2S client.
    /// </remarks>
    /// <param name="clientId">The unique identifier of the third-party S2S client that owns the setting</param>
    /// <param name="appCode">The application code identifying the platform-specific setting to be updated</param>
    /// <param name="command">The request payload containing the updated integration key and configuration details</param>
    /// <returns>Returns HTTP 204 (No Content) when the S2S client setting is successfully updated</returns>
    [HttpPut("{clientId}/settings/{appCode}")]
    public async Task<IActionResult> UpdateSettingAsync(string clientId, string appCode, UpdateS2sClientSettingCommand command)
    {
        await Mediator.Send(command with { ClientId = clientId, AppCode = appCode });
        return ApiResponseFactory.NoContent();
    }
}
