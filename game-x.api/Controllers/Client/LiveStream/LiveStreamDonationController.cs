using game_x.application.Features.LiveStreams.Streaming.Commands.DonateWithFiatCurrency;
using game_x.application.Features.LiveStreams.Streaming.Commands.DonateWithGift;

namespace game_x.api.Controllers.Client.LiveStream;

[Route("/api/livestream-donations")]
public sealed class LiveStreamDonationController : BaseApiController
{
    [Authorize(Roles = AppRoles.User)]
    [HttpPost("{streamKey}/gifts")]
    public async Task<IActionResult> DonateWithGiftAsync(string streamKey, DonateWithGiftCommand command)
    {
        await Mediator.Send(command with { StreamKey = streamKey });
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.User)]
    [HttpPost("{streamKey}/crypto-tokens")]
    public async Task<IActionResult> DonateWithCryptoAsync(string streamKey, DonateWithCryptoTokensCommand command)
    {
        await Mediator.Send(command with { StreamKey = streamKey });
        return ApiResponseFactory.NoContent();
    }
}
