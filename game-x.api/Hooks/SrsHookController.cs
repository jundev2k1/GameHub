using game_x.api.Common;
using game_x.api.Controllers;
using game_x.application.Exceptions;
using game_x.application.Features.LiveStreams.Commands.PlayStream;
using game_x.application.Features.LiveStreams.Commands.PublishStream;
using game_x.application.Features.LiveStreams.Commands.StopStream;
using game_x.application.Features.LiveStreams.Commands.UnpublishStream;
using System.Text.Json;
using System.Web;

namespace game_x.api.Hooks;

[Route("/hooks/srs")]
public sealed class SrsHookController(ILogger<SrsHookController> logger) : BaseApiController
{
    [HttpPost("on-publish")]
    public async Task<IActionResult> OnPublishAsync(SrsEventHookRequest request)
    {
        logger.LogInformation("=====SRS server=====");
        logger.LogInformation(JsonSerializer.Serialize(request));

        var query = HttpUtility.ParseQueryString(request.Param);
        var token = query.Get("token");
        if (token.IsNullOrWhiteSpace())
            throw new BadRequestException("Token is required");

        var command = new PublishStreamCommand(request.Stream, token!);
        await Mediator.Send(command);
        return Ok(0);
    }

    [HttpPost("on-unpublish")]
    public async Task<IActionResult> OnUnpublishAsync(SrsEventHookRequest request)
    {
        logger.LogInformation("=====SRS server=====");
        logger.LogInformation(JsonSerializer.Serialize(request));

        var command = new UnpublishStreamCommand(request.App, request.Stream);
        await Mediator.Send(command);
        return Ok(0);
    }

    [HttpPost("on-play")]
    public async Task<IActionResult> OnPlayAsync(SrsEventHookRequest request)
    {
        logger.LogInformation("=====SRS server=====");
        logger.LogInformation(JsonSerializer.Serialize(request));

        var query = HttpUtility.ParseQueryString(request.Param);
        var token = query.Get("token");
        if (token.IsNullOrWhiteSpace())
            throw new BadRequestException("Token is required");

        var command = new PlayStreamCommand(
            request.App,
            request.Stream,
            token!);
        await Mediator.Send(command);
        return Ok(0);
    }

    [HttpPost("on-stop")]
    public async Task<IActionResult> OnStopAsync(SrsEventHookRequest request)
    {
        logger.LogInformation("=====SRS server=====");
        logger.LogInformation(JsonSerializer.Serialize(request));
        var command = new StopStreamCommand(
            request.App,
            request.Stream,
            request.Param);
        await Mediator.Send(command);
        return Ok(0);
    }
}
