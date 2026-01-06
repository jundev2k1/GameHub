using game_x.application.Features.S2s.Commands.CreateS2sClientSetting;
using game_x.application.Features.S2s.Commands.SwitchS2sClientSettingStatus;
using game_x.application.Features.S2s.Commands.UpdateS2sClientSetting;
using game_x.application.Features.S2s.Queries.GetSettingDetail;

namespace game_x.api.Controllers.Root.S2s;

[Authorize(Roles = AppRoles.Root)]
[Route("api/root/s2s/clients")]
public sealed class S2sClientSettingController : BaseApiController
{
    /// <summary>
    /// Retrieves detailed information of an S2S client setting
    /// </summary>
    /// <remarks>
    /// This API returns the complete detail of a platform-specific setting for a third-party Server-to-Server (S2S) client
    /// <br />
    /// The response includes:
    /// <list type="number">
    ///   <item>
    ///     <description>
    ///       Parent S2S client information (third-party identity and metadata)
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       Target setting information identified by the app code
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       All associated credentials, integration keys
    ///       and related configuration materials required for server-to-server authentication
    ///     </description>
    ///   </item>
    /// </list>
    /// <br />
    /// A setting represents an integration configuration for a specific platform
    /// or channel, such as web, mobile app, or device
    /// </remarks>
    /// <param name="appCode">The application code identifying the platform-specific S2S client setting whose details will be retrieved</param>
    /// <returns>Returns HTTP 200 (OK) with the S2S client setting detail, including parent client information and all associated credentials</returns>
    [HttpGet("{clientId}/settings/{appCode}")]
    public async Task<IActionResult> GetSettingDetailAsync(string clientId, string appCode)
    {
        var query = new GetSettingDetailQuery(clientId, appCode);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

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
    /// <returns>Returns HTTP 200 when the S2S client setting is successfully updated</returns>
    [HttpPut("{clientId}/settings/{appCode}")]
    public async Task<IActionResult> UpdateSettingAsync(string clientId, string appCode, UpdateS2sClientSettingCommand command)
    {
        await Mediator.Send(command with { ClientId = clientId, AppCode = appCode });
        return ApiResponseFactory.NoContent();
    }

    /// <summary>
    /// Switches the status of an S2S client setting
    /// </summary>
    /// <remarks>
    /// This API toggles the status of a platform-specific setting of a third-party
    /// Server-to-Server (S2S) client. A setting represents an integration key and
    /// configuration for a specific platform or channel (such as web, mobile app, or device)
    /// <br />
    /// When the setting is inactive, all server-to-server authentication requests
    /// using the associated integration key will be rejected
    /// </remarks>
    /// <param name="clientId">The unique identifier of the third-party S2S client that owns the setting</param>
    /// <param name="appCode">The application code identifying the platform-specific setting whose status will be switched</param>
    /// <returns>Returns HTTP 200 when the S2S client setting status is successfully updated</returns>
    [HttpPatch("{clientId}/settings/{appCode}/status")]
    public async Task<IActionResult> SwitchS2sClientSettingStatusAsync(string clientId, string appCode)
    {
        var command = new SwitchS2sClientSettingStatusCommand(clientId, appCode);
        await Mediator.Send(command with { ClientId = clientId, AppCode = appCode });
        return ApiResponseFactory.NoContent();
    }
}
