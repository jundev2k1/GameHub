using game_x.api.Controllers;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Features.Games.WebHooks.Commands.OnWalletChanged;
using game_x.share.Settings;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace game_x.api.Hooks;

[Authorize(Policy = AppPolicies.RequireHmac)]
[Route("/hooks/games")]
public sealed class GameHookController(
    IOptions<HmacSettings> options,
    IAppLogger<GameHookController> logger) : BaseApiController
{
    [HttpPost("on-wallet-changed")]
    public async Task<IActionResult> OnWalletChangedAsync(OnWalletChangedCommand command)
    {
        logger.LogInformation("===== Game web hook =====");
        logger.LogInformation(JsonSerializer.Serialize(command));

        Request.Headers.TryGetValue(options.Value.PartnerHeader, out var partnerValues);
        var partnerName = partnerValues.First();

        await Mediator.Send(command with { PartnerName = partnerName });
        return ApiResponseFactory.NoContent();
    }
}
