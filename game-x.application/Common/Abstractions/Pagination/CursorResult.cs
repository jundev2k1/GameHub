namespace game_x.application.Common.Abstractions.Pagination;

/// <summary>Result for cursor-based pagination (no total count).</summary>
public sealed class CursorResult<TItem> where TItem : notnull
{
    public IReadOnlyList<TItem> Items { get; }
    public string? NextCursor { get; }
    public string? PrevCursor { get; }
    public bool HasMore { get; }
    public int Limit { get; }

    public CursorResult(
        IReadOnlyList<TItem> items,
        string? nextCursor,
        string? prevCursor,
        bool hasMore,
        int limit)
    {
        Items = items;
        NextCursor = nextCursor;
        PrevCursor = prevCursor;
        HasMore = hasMore;
        Limit = limit;
    }

    /// <summary>Build for next-only feeds (e.g., conversation lists).</summary>
    public static CursorResult<TItem> BuildNextOnly(
        IReadOnlyList<TItem> batchLimitPlusOne,
        int limit,
        Func<TItem, string> nextOfLastKept)
    {
        var hasMore = batchLimitPlusOne.Count > limit;
        var page = hasMore ? batchLimitPlusOne.Take(limit).ToList()
            : batchLimitPlusOne.ToList();

        var next = hasMore && page.Count > 0 ? nextOfLastKept(page[^1]) : null;

        return new CursorResult<TItem>(
            items: page,
            nextCursor: next,
            prevCursor: null,
            hasMore: hasMore,
            limit: limit);
    }

    /// <summary>Build for bidirectional feeds (e.g., messages).</summary>
    public static CursorResult<TItem> Build(
        IReadOnlyList<TItem> batchLimitPlusOne,
        int limit,
        Func<TItem, string>? nextOfLastKept,
        Func<TItem, string>? prevOfFirstKept,
        bool emitPrev)
    {
        var hasMore = batchLimitPlusOne.Count > limit;
        var page = hasMore ? batchLimitPlusOne.Take(limit).ToList()
            : batchLimitPlusOne.ToList();

        string? next = null, prev = null;

        if (page.Count > 0)
        {
            if (hasMore && nextOfLastKept is not null)
                next = nextOfLastKept(page[^1]);

            if (emitPrev && prevOfFirstKept is not null)
                prev = prevOfFirstKept(page[0]);
        }

        return new CursorResult<TItem>(
            items: page,
            nextCursor: next,
            prevCursor: prev,
            hasMore: hasMore,
            limit: limit);
    }
}
