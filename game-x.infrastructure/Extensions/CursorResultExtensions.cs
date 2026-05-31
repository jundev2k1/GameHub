using game_x.application.Common.Abstractions.Pagination;

namespace game_x.infrastructure.Extensions;

public static class CursorResultExtensions
{
    public static CursorResult<TOut> Transform<TIn, TOut>(
        this CursorResult<TIn> src,
        IEnumerable<TOut> newItems)
        where TIn : notnull
        where TOut : notnull
    {
        return new CursorResult<TOut>(
            items: newItems.ToList(),
            nextCursor: src.NextCursor,
            prevCursor: src.PrevCursor,
            limit: src.Limit
        );
    }
}
