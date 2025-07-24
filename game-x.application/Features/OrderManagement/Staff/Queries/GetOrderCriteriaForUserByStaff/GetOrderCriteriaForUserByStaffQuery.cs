using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;

namespace game_x.application.Features.OrderManagement.Staff.Queries.GetOrderCriteriaForUserByStaff;

public record GetOrderCriteriaForUserByStaffQuery(
    string UserId,
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int? PageIndex,
    int? PageSize) : IQuery<PaginationResult<GetOrderOfUserByCriteriaResult>>;

public record GetOrderOfUserByCriteriaResult(
    Guid OrderId,
    string UxmOrderId,
    string StaffId,
    string OrderType,
    decimal FiatAmount,
    decimal CryptoAmount,
    FiatType? FiatType,
    CryptoType? CryptoType,
    string OrderStatus,
    string? EntryCode,
    DateTime CreatedAt);
    