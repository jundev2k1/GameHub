using game_x.application.Common.Filters;
using game_x.application.Features.CounterManagement.Dtos;

namespace game_x.application.Features.CounterManagement.Admin.Queries.GetCounterDetailStatistics;

public record GetCounterStatisticDetailQuery(Guid CounterId, IEnumerable<QueryFilter> Filters) : IQuery<CounterStatisticsDto>;
