using game_x.api.Dtos;
using game_x.application.Features.LiveStreams.Reminders.Commands.SubscribeStream;
using game_x.application.Features.LiveStreams.Reminders.Commands.UpdateStreamReminders;
using game_x.application.Features.LiveStreams.Streaming.Commands.JoinLiveStream;
using game_x.application.Features.LiveStreams.Streaming.Queries.GetChatMessageInStream;

namespace game_x.api.Controllers.Client.LiveStream;

[Route("/api/user/livestreams")]
public sealed class LiveStreamController : BaseApiController
{
    [Authorize(Roles = $"{AppRoles.Talent},{AppRoles.User},{AppRoles.Cs},{AppRoles.Admin}")]
    [HttpPost("{id:guid}/join")]
    public async Task<IActionResult> JoinLiveStreamAsync(Guid id)
    {
        var query = new JoinLiveStreamCommand(id);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = $"{AppRoles.Talent},{AppRoles.User}")]
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

    [Authorize(Roles = AppRoles.User)]
    [HttpPost("{streamId:guid}/reminders")]
    public async Task<IActionResult> SubscribeStreamAsync(Guid streamId, SubscribeStreamCommand command)
    {
        await Mediator.Send(command with { StreamId = streamId });
        return ApiResponseFactory.Created();
    }

    [Authorize(Roles = AppRoles.User)]
    [HttpPut("{streamId:guid}/reminders")]
    public async Task<IActionResult> UpdateStreamRemainderAsync(Guid streamId, UpdateStreamRemindersCommand command)
    {
        await Mediator.Send(command with { StreamId = streamId });
        return ApiResponseFactory.NoContent();
    }
}
