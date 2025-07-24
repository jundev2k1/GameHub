using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.CounterManagement.Dtos;
using game_x.application.Mappers.Counter;

namespace game_x.application.Features.CounterManagement.Admin.Queries.GetCounterCriteriaByAdmin;

public sealed class GetCounterCriteriaByAdminHandler(
    ICriteriaBuilder<Counter> builder,
    CounterMapper counterMapper,
    ICounterRepo counterRepo) : IQueryHandler<GetCounterCriteriaByAdminQuery, PaginationResult<CounterDto>>
{
    public async Task<PaginationResult<CounterDto>> Handle(GetCounterCriteriaByAdminQuery request, CancellationToken ct = default)
    {
        var items = await counterRepo.GetByCriteriaAsync(
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword =>
                    counter => counter.CounterName.ToLowerInvariant()!.Contains(keyword.ToLowerInvariant())),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);
        var result = counterMapper.ToCounterDtos(items);
        return result;
    }
}
