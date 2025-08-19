using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.Chat.Commands.SendSupportMessage;
using game_x.application.Features.Chat.Queries.ListUnassignedQueue;

namespace game_x.api.Controllers.Chat;

[Route("api/support")]
public class SupportController : BaseApiController
{
    [Authorize(Roles = AppRoles.User)]
    [HttpPost("messages")]
    public async Task<IActionResult> SendMessageAsync([FromBody] SendSupportMessageCommand command, CancellationToken ct)
    {
        var result = await Mediator.Send(command, ct);
        return ApiResponseFactory.Ok(result);
    }
    
    /// <summary>
    /// List unassigned support conversations (ordered by lastMessageAt desc).
    /// Cursor-based pagination. Optional search by customer or last message text.
    /// </summary>
    [Authorize(Roles = AppRoles.User)]
    [HttpGet("queue/unassigned")]
    public async Task<IActionResult> GetUnassignedConversationAsync([AsParameters] CursorCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters);
        var query = new ListUnassignedQueueQuery(
            Filters: filters,
            Q: parameters.Q,
            Search: parameters.Search,
            Limit: parameters.Limit,
            Cursor: parameters.Cursor
        );
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
}