using game_x.api.Common;
using game_x.api.Controllers;
using System.Text.Json;

namespace game_x.api.Hooks;

[Route("/hooks/srs")]
public sealed class SrsHookController(ILogger<SrsHookController> logger) : BaseApiController
{
    [HttpPost("on-publish")]
    public async Task<IActionResult> OnPublishAsync(SrsEventHookRequest request)
    {
        logger.LogInformation("=====SRS server=====");
        logger.LogInformation(JsonSerializer.Serialize(request));
        await Task.CompletedTask;
        return Ok(0);
    }

    [HttpPost("on-unpublish")]
    public async Task<IActionResult> OnUnpublishAsync(SrsEventHookRequest request)
    {
        logger.LogInformation("=====SRS server=====");
        logger.LogInformation(JsonSerializer.Serialize(request));
        await Task.CompletedTask;
        return Ok(0);
    }

    [HttpPost("on-play")]
    public async Task<IActionResult> OnPlayAsync(SrsEventHookRequest request)
    {
        logger.LogInformation("=====SRS server=====");
        logger.LogInformation(JsonSerializer.Serialize(request));
        await Task.CompletedTask;
        return Ok(0);
    }

    [HttpPost("on-stop")]
    public async Task<IActionResult> OnStopAsync(SrsEventHookRequest request)
    {
        logger.LogInformation("=====SRS server=====");
        logger.LogInformation(JsonSerializer.Serialize(request));
        await Task.CompletedTask;
        return Ok(0);
    }
}
