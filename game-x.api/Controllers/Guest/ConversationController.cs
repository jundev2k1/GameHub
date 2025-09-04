namespace game_x.api.Controllers.Guest;

[Route("api/guest")]
public class ConversationController : BaseApiController
{
    /// <summary>List conversations for current logged-in user</summary>
    [AllowAnonymous]
    [HttpPost("start")]
    public async Task<IActionResult> StartGuest()
    {
        var guestId = Guid.NewGuid().ToString("N");
        return ApiResponseFactory.Ok(guestId);
    }
}