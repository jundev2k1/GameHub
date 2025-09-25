using game_x.application.Features.LiveStreams.Gifts.Queries.GetAllActiveGifts;

namespace game_x.api.Controllers.Client.LiveStream;

[Route("/api/user/livestream-gifts")]
public sealed class LiveStreamGiftController : BaseApiController
{
    [Authorize(Roles = AppRoles.User)]
    [HttpGet]
    public async Task<IActionResult> GetGiftsAsync()
    {
        var result = await Mediator.Send(new GetAllActiveGiftsQuery());
        return ApiResponseFactory.Ok(result);
    }
}
