using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Features.Friends.Dtos;

namespace game_x.application.Features.Friends.Queries.GetOutgoingFriendRequests;

public sealed record GetOutgoingFriendRequestsQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int? PageIndex,
    int? PageSize) : IQuery<PaginationResult<OutgoingFriendRequestDto>>;