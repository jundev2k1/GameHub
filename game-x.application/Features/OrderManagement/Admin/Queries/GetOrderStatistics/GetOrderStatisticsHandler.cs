using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.OrderManagement.Dtos;

namespace game_x.application.Features.OrderManagement.Admin.Queries.GetOrderStatistics;

public sealed class GetOrderStatisticsHandler(IOrderRepo orderRepo) : IQueryHandler<GetOrderStatisticsQuery, OrderStatisticsDto>
{
    public async Task<OrderStatisticsDto> Handle(GetOrderStatisticsQuery request, CancellationToken ct = default)
    {
        var statistics = await orderRepo.GetOrderStatisticsAsync(ct);
        return statistics;
    }
}
