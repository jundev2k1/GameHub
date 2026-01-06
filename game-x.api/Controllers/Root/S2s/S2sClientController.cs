using game_x.application.Features.S2s.Commands.CreateS2sClient;

namespace game_x.api.Controllers.Root.S2s;

[Authorize(Roles = AppRoles.Root)]
[Route("api/root/s2s")]
public sealed class S2sClientController : BaseApiController
{
    /// <summary>
    /// Creates a new Server-to-Server (S2S) client configuration.
    /// </summary>
    /// <remarks>
    /// This API registers a new S2S client used for server-to-server authentication
    /// and secure communication between external applications and the system.
    /// </remarks>
    /// <param name="command">The request payload containing S2S client information</param>
    /// <returns>Returns HTTP 201 (Created) when the S2S client is successfully created.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateS2sClientAsync(CreateS2sClientCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.Created();
    }
}
