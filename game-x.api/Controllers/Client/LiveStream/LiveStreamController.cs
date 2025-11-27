using game_x.api.Dtos;
using game_x.application.Features.LiveStreams.Streaming.Commands.JoinLiveStream;
using game_x.application.Features.LiveStreams.Streaming.Queries.GetChatMessageInStream;

namespace game_x.api.Controllers.Client.LiveStream;

[Authorize(Roles = $"{AppRoles.Talent},{AppRoles.User}")]
[Route("/api/user/livestreams")]
public sealed class LiveStreamController : BaseApiController
{
    [HttpPost("{id:guid}/join")]
    public async Task<IActionResult> JoinLiveStreamAsync(Guid id)
    {
        var query = new JoinLiveStreamCommand(id);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("{streamKey}/chats")]
    public async Task<IActionResult> GetChatsInStreamAsync(string streamKey, [FromQuery] GetChatMessageInStreamRequest request)
    {
        var query = new GetChatMessageInStreamQuery(
            streamKey,
            request.MessageId,
            request.IsNext,
            request.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
}
