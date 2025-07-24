using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Extensions.FilterExtensions;
using game_x.application.Features.OrderManagement.Dtos;
using game_x.application.Mappers.Order;

namespace game_x.application.Features.OrderManagement.Client.Queries.GetOrderCriteriaByClient;

public sealed class GetOrderCriteriaByClientHandler(
    ICriteriaBuilder<Order> builder,
    OrderMapper orderMapper,
    IOrderRepo orderRepo,
    IUserAccessor userAccessor) : IQueryHandler<GetOrderCriteriaByClientQuery, PaginationResult<GetOrderCriteriaByClientResult>>
{
    public async Task<PaginationResult<GetOrderCriteriaByClientResult>> Handle(
        GetOrderCriteriaByClientQuery request,
        CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();

        var items = await orderRepo.GetOrderCriteriaByClientAsync(
            userId,
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
            item => item.Adapt<OrderDto>().Adapt<GetOrderCriteriaByClientResult>());
        return result;
    }
}