using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.CounterManagement.Dtos;

namespace game_x.application.Features.CounterManagement.Admin.Queries.GetCounterStatistics;

public sealed class GetCounterStatisticsHandler(ICriteriaBuilder<Order> builder, ICounterRepo counterRepo)
    : IQueryHandler<GetCounterStatisticsQuery, CounterStatisticsDto>
{
    public async Task<CounterStatisticsDto> Handle(GetCounterStatisticsQuery request, CancellationToken ct = default)
    {
        var statistics = await counterRepo.GetCounterStatisticsAsync(query => builder.Apply(
            query,
            request.Filters), ct);
        return statistics;
    }
}