using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Extensions.FilterExtensions;
using game_x.application.Features.OrderManagement.Dtos;
using game_x.application.Mappers.Order;

namespace game_x.application.Features.OrderManagement.Admin.Queries.GetOrderCriteriaByAdmin;

public sealed class GetOrderCriteriaByAdminHandler(ICriteriaBuilder<Order> builder, OrderMapper orderMapper, IOrderRepo orderRepo)
    : IQueryHandler<GetOrderCriteriaByAdminQuery, PaginationResult<GetOrderCriteriaByAdminResult>>
{
    public async Task<PaginationResult<GetOrderCriteriaByAdminResult>> Handle(GetOrderCriteriaByAdminQuery request, CancellationToken ct = default)
    {
        var items = await orderRepo.GetByCriteriaAsync(
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword =>
                    order => order.User.UserName!.StartsWith(keyword), OrderFilterExtensions.Options),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);
        var result = orderMapper.ToSearchResult(
            items,
            item => item.Adapt<OrderDto>().Adapt<GetOrderCriteriaByAdminResult>());
        return result;
    }
}