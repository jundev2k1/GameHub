using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.Friends.Commands.RespondFriendRequest;
using game_x.application.Features.Friends.Commands.SendFriendRequest;
using game_x.application.Features.Friends.Queries.FriendSearch;
using game_x.application.Features.Friends.Queries.GetFriendships;
using game_x.application.Features.Friends.Queries.GetIncomingFriendRequests;
using game_x.application.Features.Friends.Queries.GetOutgoingFriendRequests;

namespace game_x.api.Controllers.Client.Chat;

[Authorize(Roles = AppRoles.User)]
[Route("api/user")]
public class FriendController : BaseApiController
{
    [HttpGet("friendships/search")]
    public async Task<IActionResult> SearchFriendsAsync([AsParameters] SearchCriteriaRequest parameters)
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
    
    [HttpGet("friendships")]
    public async Task<IActionResult> GetFriendshipsAsync([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetFriendshipsQuery(
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
    
    /// <summary>Retrieve the list of requests sent to and awaiting response from the logged-in user.</summary>
    [HttpGet("friend-requests/incoming")]
    public async Task<IActionResult> GetReceivedFriendRequestsAsync([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetIncomingFriendRequestsQuery(
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
    
    /// <summary>Retrieve the list of requests sent by the logged-in user.</summary>
    [HttpGet("friend-requests/outgoing")]
    public async Task<IActionResult> GetSentFriendRequestsAsync([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetOutgoingFriendRequestsQuery(
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("friend-requests")]
    public async Task<IActionResult> SendRequestAsync([FromBody] SendFriendRequestCommand cmd)
    {
        var result = await Mediator.Send(cmd);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("friend-requests/{linkPublicId:guid}/respond")]
    public async Task<IActionResult> RespondAsync(Guid linkPublicId, [FromBody] RespondFriendRequestCommand cmd)
    {
        cmd.LinkPublicId = linkPublicId;
        var result = await Mediator.Send(cmd);
        return ApiResponseFactory.Ok(result);
    }
}