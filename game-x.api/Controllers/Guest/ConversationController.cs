using game_x.api.Common;
using game_x.application.Features.Chat.Queries.ListMessagesInConversation;
using game_x.application.Features.Chat.Queries.ListMyConversationsForGuest;

namespace game_x.api.Controllers.Guest;

[Route("api/guest")]
[AllowAnonymous]
public class ConversationController : BaseApiController
{
    /// <summary>List conversations for current logged-in user</summary>
    [HttpPost("start")]
    public IActionResult StartGuest()
    {
        var guestId = Guid.NewGuid().ToString("N");
        return ApiResponseFactory.Ok(guestId);
    }
    
    /// <summary>
    /// List conversations for current logged-in user
    /// </summary>
    [HttpGet("conversations/me")]
    public async Task<IActionResult> GetSupportConversationAsync([FromHeader] string guestId)
    {
        var query = new ListMyConversationsForGuestQuery(GuestId: guestId);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpGet("conversations/{convId:guid}/messages")]
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
}