using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Extensions.FilterExtensions;
using game_x.application.Features.OrderManagement.Dtos;
using game_x.application.Mappers.Order;

namespace game_x.application.Features.OrderManagement.Staff.Queries.GetOrderCriteriaForUserByStaff;

public sealed class
    GetOrderCriteriaForUserByStaffHandler(OrderMapper orderMapper, ICriteriaBuilder<Order> builder, IOrderRepo orderRepo)
    : IQueryHandler<GetOrderCriteriaForUserByStaffQuery, PaginationResult<GetOrderOfUserByCriteriaResult>>
{
    public async Task<PaginationResult<GetOrderOfUserByCriteriaResult>> Handle(GetOrderCriteriaForUserByStaffQuery request,
        CancellationToken ct = default)
    {
        var items = await orderRepo.GetOrderCriteriaByClientAsync(
            request.UserId,
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
            item => item.Adapt<OrderDto>().Adapt<GetOrderOfUserByCriteriaResult>());
        return result;
    }
}