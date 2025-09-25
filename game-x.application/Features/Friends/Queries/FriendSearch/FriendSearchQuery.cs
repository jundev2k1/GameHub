using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Features.Friends.Dtos;

namespace game_x.application.Features.Friends.Queries.FriendSearch;

public record FriendSearchQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int? PageIndex,
    int? PageSize) : IQuery<PaginationResult<FriendSearchResultDto>>;