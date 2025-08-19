using System.Linq.Expressions;
using game_x.application.Common.Abstractions.Pagination;

namespace game_x.application.Common.Filters;

/// <summary>Fluent builder for opaque seeks pagination (2-key: UTC DateTime + int).</summary>
public interface ISeekCursorBuilder<TModel> where TModel : notnull
{
    /// <summary>Set ordering keys.</summary>
    ISeekCursorBuilder<TModel> Keys(
        Expression<Func<TModel, DateTime>> key1,
        Expression<Func<TModel, int>> key2);

    /// <summary>Set canonical sort order.</summary>
    ISeekCursorBuilder<TModel> Sort(bool desc1, bool desc2);

    /// <summary>Apply a cursor token (optionally with a filter fingerprint).</summary>
    ISeekCursorBuilder<TModel> FromCursor(string? cursor, string? fp = null);

    /// <summary>Enable prevCursor generation.</summary>
    ISeekCursorBuilder<TModel> WithPrev(bool enable = true);

    ISeekCursorBuilder<TModel> Limit(int size);

    /// <summary>Execute query and project to DTO.</summary>
    Task<CursorResult<TDto>> ExecuteAsync<TDto>(
        Func<TModel, TDto> map,
        CancellationToken ct = default)
        where TDto : notnull;
}