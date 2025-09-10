using game_x.api.Controllers;
using game_x.api.Dtos.SrsHooks;
using System.Text.Json;

namespace game_x.api.Hooks;

[Route("/hooks/srs")]
public sealed class SrsHookController(ILogger<SrsHookController> logger) : BaseApiController
{
    [HttpPost("on-publish")]
    public async Task<IActionResult> OnPublishAsync(OnPublishRequest request)
    {
        logger.LogInformation("=====SRS server=====");
        logger.LogInformation(JsonSerializer.Serialize(request));
        await Task.CompletedTask;
        return Ok();
    }

    [HttpPost("on-unpublish")]
    public async Task<IActionResult> OnUnpublishAsync(OnUnpublishRequest request)
    {
        logger.LogInformation("=====SRS server=====");
        logger.LogInformation(JsonSerializer.Serialize(request));
        await Task.CompletedTask;
        return Ok();
    }

    [HttpPost("on-connect")]
    public async Task<IActionResult> OnConnectAsync(OnConnectRequest request)
    {
        logger.LogInformation("=====SRS server=====");
        logger.LogInformation(JsonSerializer.Serialize(request));
        await Task.CompletedTask;
        return Ok();
    }

    [HttpPost("on-disconnect")]
    public async Task<IActionResult> OnDisconnectAsync(OnDisconnectRequest request)
    {
        logger.LogInformation("=====SRS server=====");
        logger.LogInformation(JsonSerializer.Serialize(request));
        await Task.CompletedTask;
        return Ok();
    }

    [HttpPost("on-play")]
    public async Task<IActionResult> OnPlayAsync(OnPlayRequest request)
    {
        logger.LogInformation("=====SRS server=====");
        logger.LogInformation(JsonSerializer.Serialize(request));
        await Task.CompletedTask;
        return Ok();
    }

    [HttpPost("on-stop")]
    public async Task<IActionResult> OnStopAsync(OnStopRequest request)
    {
        logger.LogInformation("=====SRS server=====");
        logger.LogInformation(JsonSerializer.Serialize(request));
        await Task.CompletedTask;
        return Ok();
    }
}
