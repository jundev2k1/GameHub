using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.Chat.Queries.ListMyConversations;

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
}