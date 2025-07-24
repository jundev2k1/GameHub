using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Features.OrderManagement.Dtos;

namespace game_x.application.Features.OrderManagement.Admin.Queries.GetOrderCriteriaByAdmin;

public record GetOrderCriteriaByAdminQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int? PageIndex,
    int? PageSize) : IQuery<PaginationResult<GetOrderCriteriaByAdminResult>>;


public record GetOrderCriteriaByAdminResult(
    Guid OrderId,
    string UxmOrderId,
    string OrderType,
    decimal FiatAmount,
    decimal CryptoAmount,
    PricingMode PricingMode,
    FiatType? FiatType,
    CryptoType? CryptoType,
    string OrderStatus,
    decimal UxmFee,
    DateTime CreatedAt);