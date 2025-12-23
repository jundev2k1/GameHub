using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.UserGameSessions.Dtos;

namespace game_x.application.Features.UserGameSessions.Queries.GetGameSessions;

public sealed class GetGameSessionsHandler(
    ICriteriaBuilder<UserGameSessionSearchItemDto> criteriaBuilder,
    IUserGameSessionRepo userGameSessionRepo) : IQueryHandler<GetGameSessionsQuery, PaginationResult<UserGameSessionSearchItemDto>>
{
    public async Task<PaginationResult<UserGameSessionSearchItemDto>> Handle(GetGameSessionsQuery request, CancellationToken ct = default)
    {
        var data = await userGameSessionRepo.GetsByCriteriaAsync(
            query => criteriaBuilder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword => ugs => ugs.Nickname.Contains(keyword)
                    || ugs.PlatformName.Contains(keyword)
                    || (ugs.GameName != null && ugs.GameName.Contains(keyword))),
            request.PageIndex,
            request.PageSize,
            ct);
        return data;
    }
}
