using game_x.api.Dtos;
using game_x.application.Features.LiveStreams.Commands.JoinLiveStream;
using game_x.application.Features.LiveStreams.Queries.GetChatMessageInStream;

namespace game_x.api.Controllers.Client.LiveStream;

[Route("/api/user/livestreams")]
public sealed class LiveStreamController : BaseApiController
{
    [Authorize(Roles = AppRoles.User)]
    [HttpPost("{id:guid}/join")]
    public async Task<IActionResult> JoinLiveStreamAsync(Guid id)
    {
        var query = new JoinLiveStreamCommand(id);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = AppRoles.User)]
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
