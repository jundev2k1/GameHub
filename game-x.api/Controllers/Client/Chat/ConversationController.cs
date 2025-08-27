using game_x.api.Common;
using game_x.application.Features.Chat.Queries.ListMessagesInConversation;
using game_x.application.Features.Chat.Queries.ListMyConversationsForClient;
using game_x.application.Features.Chat.Queries.ListWindowMessagesInConversation;

namespace game_x.api.Controllers.Client.Chat;

[Authorize(Roles = AppRoles.User)]
[Route("api/user/conversations")]
public class ConversationController : BaseApiController
{
    /// <summary>
    /// List conversations for current logged-in user
    /// </summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMyConversationAsync([AsParameters] CursorCriteriaRequest parameters)
    {
        var query = new ListMyConversationsForClientQuery(
            Limit: parameters.Limit,
            Cursor: parameters.Cursor
        );
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("{convId:guid}/messages")]
    public async Task<IActionResult> ListMessagesInConversationAsync(
        Guid convId,
        [AsParameters] CursorCriteriaRequest parameters)
    {
        var query = new ListMessagesInConversationQuery(
            ConvId: convId,
            Limit: parameters.Limit,
            Cursor: parameters.Cursor
        );
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpGet("{convId:guid}/messages/{anchorId:guid}/window")]
    public async Task<IActionResult> ListWindowMessagesInConversationAsync(
        Guid convId,
        Guid anchorId,
        [FromQuery] int before = 30,
        [FromQuery] int after = 30,
        [FromQuery] string anchor = "self")
    {
        var query = new ListWindowMessagesInConversationQuery(
            ConvId: convId,
            AnchorId: anchorId,
            Before: before,
            After: after,
            Anchor: anchor
        );
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
}