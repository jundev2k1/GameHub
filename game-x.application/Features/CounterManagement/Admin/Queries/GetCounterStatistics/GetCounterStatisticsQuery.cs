using game_x.application.Common.Filters;
using game_x.application.Features.CounterManagement.Dtos;

namespace game_x.application.Features.CounterManagement.Admin.Queries.GetCounterStatistics;

public record GetCounterStatisticsQuery(IEnumerable<QueryFilter> Filters): IQuery<CounterStatisticsDto>;