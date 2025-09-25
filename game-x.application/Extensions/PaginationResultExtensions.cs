using game_x.application.Common.Abstractions.Pagination;

namespace game_x.application.Extensions;

public static class PaginationExtensions
{
    public static PaginationResult<TOut> Transform<TIn, TOut>(
        this PaginationResult<TIn> src,
        IEnumerable<TOut> newItems)
        where TIn : class
        where TOut : class
    {
        return new PaginationResult<TOut>(
            items: newItems.ToList(),
            totalItems: src.TotalItems,
            totalPages: src.TotalPages,
            pageSize: src.PageSize,
            pageIndex: src.PageNumber
        );
    }
}
