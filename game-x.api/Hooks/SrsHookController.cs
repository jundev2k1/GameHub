using game_x.api.Controllers;
using System.Text.Json;

namespace game_x.api.Hooks;

[Route("/hooks/srs")]
public sealed class SrsHookController(ILogger<SrsHookController> logger) : BaseApiController
{
    [HttpPost("on-publish")]
    public async Task<IActionResult> OnPublishAsync(SrsPublishRequest request)
    {
        logger.LogInformation("Called OnPublish successfully.");
        logger.LogInformation(JsonSerializer.Serialize(request));
        await Task.CompletedTask;
        return Ok();
    }

    [HttpPost("on-unpublish")]
    public async Task<IActionResult> OnUnpublishAsync(SrsPublishRequest request)
    {
        logger.LogInformation("Called OnUnpublish successfully.");
        logger.LogInformation(JsonSerializer.Serialize(request));
        await Task.CompletedTask;
        return Ok();
    }

    [HttpPost("on-connect")]
    public async Task<IActionResult> OnConnectAsync(SrsPublishRequest request)
    {
        logger.LogInformation("Called OnConnect successfully.");
        logger.LogInformation(JsonSerializer.Serialize(request));
        await Task.CompletedTask;
        return Ok();
    }

    [HttpPost("on-disconnect")]
    public async Task<IActionResult> OnDisconnectAsync(SrsPublishRequest request)
    {
        logger.LogInformation("Called OnDisconnect successfully.");
        logger.LogInformation(JsonSerializer.Serialize(request));
        await Task.CompletedTask;
        return Ok();
    }

    [HttpPost("on-play")]
    public async Task<IActionResult> OnPlayAsync(SrsPublishRequest request)
    {
        logger.LogInformation("Called OnPlay successfully.");
        logger.LogInformation(JsonSerializer.Serialize(request));
        await Task.CompletedTask;
        return Ok();
    }

    [HttpPost("on-stop")]
    public async Task<IActionResult> OnStopAsync(SrsPublishRequest request)
    {
        logger.LogInformation("Called OnStop successfully.");
        logger.LogInformation(JsonSerializer.Serialize(request));
        await Task.CompletedTask;
        return Ok();
    }
}

public class SrsPublishRequest
{
    public string Action { get; set; } = default!;
    public string ClientId { get; set; } = default!;
    public string Stream { get; set; } = default!;
    public string App { get; set; } = default!;
    public string Ip { get; set; } = default!;
}