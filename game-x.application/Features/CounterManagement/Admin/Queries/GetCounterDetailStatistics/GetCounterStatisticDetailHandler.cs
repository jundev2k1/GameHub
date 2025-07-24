using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.CounterManagement.Dtos;

namespace game_x.application.Features.CounterManagement.Admin.Queries.GetCounterDetailStatistics;

public sealed class GetCounterStatisticDetailHandler(ICriteriaBuilder<Order> builder, ICounterRepo counterRepo)
    : IQueryHandler<GetCounterStatisticDetailQuery, CounterStatisticsDto>
{
    public async Task<CounterStatisticsDto> Handle(GetCounterStatisticDetailQuery request,
        CancellationToken ct = default)
    {
        var isExistCounter = await counterRepo.IsExistCounterAsync(request.CounterId, ct);
        if (!isExistCounter)
            throw new NotFoundException(nameof(Counter), nameof(Counter.PublicId));
        
        var statistics = await counterRepo.GetCounterStatisticDetailAsync(
            request.CounterId, 
            query => builder.Apply(
            query,
            request.Filters), 
            ct);
        return statistics;
    }
}
