using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.Chat.Queries.ListMessagesInConversation;
using game_x.application.Features.Chat.Queries.ListMyConversations;
using game_x.application.Features.Chat.Queries.ListWindowMessagesInConversation;

namespace game_x.api.Controllers.Chat;

[Route("api/conversations")]
public class ConversationController : BaseApiController
{
    /// <summary>
    /// List conversations for current logged-in user
    /// </summary>
    [Authorize(Roles = $"{AppRoles.Root},{AppRoles.Admin},{AppRoles.Cs},{AppRoles.User}")]
    [HttpGet("me")]
    public async Task<IActionResult> GetUnassignedConversationAsync([AsParameters] CursorCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters);
        var query = new ListMyConversationsQuery(
            Filters: filters,
            Q: parameters.Q,
            Search: parameters.Search,
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