using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.Friends.Commands.Block;
using game_x.application.Features.Friends.Commands.RespondFriendRequest;
using game_x.application.Features.Friends.Commands.SendFriendRequest;
using game_x.application.Features.Friends.Commands.Unblock;
using game_x.application.Features.Friends.Commands.Unfriend;
using game_x.application.Features.Friends.Queries.CheckFriendSocialLink;
using game_x.application.Features.Friends.Queries.FriendSearch;
using game_x.application.Features.Friends.Queries.GetBlockedFriends;
using game_x.application.Features.Friends.Queries.GetFriendships;
using game_x.application.Features.Friends.Queries.GetIncomingFriendRequests;
using game_x.application.Features.Friends.Queries.GetOutgoingFriendRequests;

namespace game_x.api.Controllers.Client.Chat;

[Authorize(Roles = AppRoles.User)]
[Route("api/user")]
public class FriendController : BaseApiController
{
    [HttpGet("friendships/{userId}/check")]
    public async Task<IActionResult> FriedCheckAsync(string userId)
    {
        var result = await Mediator.Send(new CheckFriendSocialLinkQuery(userId));
        return ApiResponseFactory.Ok(result);
    }
    
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
    
    [HttpPost("friend-requests/{linkId:guid}/respond")]
    public async Task<IActionResult> RespondAsync(Guid linkId, [FromBody] RespondFriendRequestCommand cmd)
    {
        cmd.LinkPublicId = linkId;
        var result = await Mediator.Send(cmd);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("friendships/unfriend")]
    public async Task<IActionResult> UnfriendAsync([FromBody] UnfriendCommand cmd)
    {
        var result = await Mediator.Send(cmd);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpGet("friendships/block")]
    public async Task<IActionResult> GetBlockedFriendsAsync([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetBlockedFriendsQuery(
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("friendships/block")]
    public async Task<IActionResult> BlockAsync([FromBody] BlockCommand cmd)
    {
        var result = await Mediator.Send(cmd);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("friendships/unblock")]
    public async Task<IActionResult> BlockAsync([FromBody] UnblockCommand cmd)
    {
        var result = await Mediator.Send(cmd);
        return ApiResponseFactory.Ok(result);
    }
}