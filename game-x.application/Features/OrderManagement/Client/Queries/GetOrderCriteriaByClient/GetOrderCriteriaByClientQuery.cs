using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;

namespace game_x.application.Features.OrderManagement.Client.Queries.GetOrderCriteriaByClient;

public record GetOrderCriteriaByClientQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int? PageIndex,
    int? PageSize) : IQuery<PaginationResult<GetOrderCriteriaByClientResult>>;

public record GetOrderCriteriaByClientResult(
    Guid OrderId,
    string UxmOrderId,
    string OrderType,
    decimal FiatAmount,
    decimal CryptoAmount,
    FiatType? FiatType,
    CryptoType? CryptoType,
    string OrderStatus,
    DateTime CreatedAt,
    DateTime UpdatedAt);