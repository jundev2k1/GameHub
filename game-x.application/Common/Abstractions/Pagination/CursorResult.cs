namespace game_x.application.Common.Abstractions.Pagination;

/// <summary>Result for cursor-based pagination (no total count).</summary>
public sealed class CursorResult<TItem> where TItem : notnull
{
    public IReadOnlyList<TItem> Items { get; }
    public string? NextCursor { get; }
    public string? PrevCursor { get; }
    public int Limit { get; }

    public CursorResult(
        IReadOnlyList<TItem> items,
        string? nextCursor,
        string? prevCursor,
        int limit)
    {
        Items = items;
        NextCursor = nextCursor;
        PrevCursor = prevCursor;
        Limit = limit;
    }
}
