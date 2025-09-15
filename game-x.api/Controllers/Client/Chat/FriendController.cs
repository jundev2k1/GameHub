using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.Friends.Commands.RespondFriendRequest;
using game_x.application.Features.Friends.Commands.SendFriendRequest;
using game_x.application.Features.Friends.Queries.FriendSearch;
using game_x.application.Features.Friends.Queries.GetFriendRequests;

namespace game_x.api.Controllers.Client.Chat;

[Authorize(Roles = AppRoles.User)]
[Route("api/user/friends")]
public class FriendController : BaseApiController
{
    [HttpGet("search")]
    public async Task<IActionResult> GetMyConversationAsync([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new FriendSearchQuery(
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
    
    /// <summary>Retrieve the list of requests sent to and awaiting response from the logged-in user.</summary>
    [HttpGet("requests")]
    public async Task<IActionResult> GetFriendRequestsAsync([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetFriendRequestsQuery(
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("requests")]
    public async Task<IActionResult> SendRequestAsync([FromBody] SendFriendRequestCommand cmd)
    {
        var result = await Mediator.Send(cmd);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("requests/{linkPublicId:guid}/respond")]
    public async Task<IActionResult> RespondAsync(Guid linkPublicId, [FromBody] RespondFriendRequestCommand cmd)
    {
        cmd.LinkPublicId = linkPublicId;
        var result = await Mediator.Send(cmd);
        return ApiResponseFactory.Ok(result);
    }
}