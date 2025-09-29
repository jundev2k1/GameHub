using game_x.application.Features.LiveStreams.Streaming.Commands.DonateWithGift;

namespace game_x.api.Controllers.Client.LiveStream;

[Route("/api/livestream-donations")]
public sealed class LiveStreamDonationController : BaseApiController
{
    [HttpPost("{streamKey}/gifts")]
    public async Task<IActionResult> DonateWithGiftAsync(string streamKey, DonateWithGiftCommand command)
    {
        await Mediator.Send(command with { StreamKey = streamKey });
        return ApiResponseFactory.NoContent();
    }
}
