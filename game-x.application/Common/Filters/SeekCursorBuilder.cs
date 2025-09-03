using System.Linq.Expressions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.share.Helper;
using Microsoft.EntityFrameworkCore;

namespace game_x.application.Common.Filters;

public sealed class SeekCursorBuilder<T> : ISeekCursorBuilder<T> where T : notnull
{
    private IQueryable<T> _src;
    private Expression<Func<T, DateTime>> _key1 = null!;
    private Expression<Func<T, int>> _key2 = null!;
    private bool _desc1, _desc2;
    private CursorHelper.Payload? _pivot;
    private int _limit = 50;
    private bool _withPrev;
    private Func<T, DateTime> _key1C = null!;
    private Func<T, int> _key2C = null!;
    private string? _fp;

    private SeekCursorBuilder(IQueryable<T> src) { _src = src; }
    public static SeekCursorBuilder<T> For(IQueryable<T> src) => new(src);

    public ISeekCursorBuilder<T> Keys(Expression<Func<T, DateTime>> key1, Expression<Func<T, int>> key2)
    {
        _key1 = key1;
        _key2 = key2;
        _key1C = key1.Compile();
        _key2C = key2.Compile();
        return this;
    }

    public ISeekCursorBuilder<T> Sort(bool desc1, bool desc2)
    {
        _desc1 = desc1; 
        _desc2 = desc2;
        return this;
    }

    public ISeekCursorBuilder<T> FromCursor(string? cursor, string? fp = null)
    {
        _fp = fp;
        if (CursorHelper.Token.TryDecode(cursor, out var p))
        {
            if (_fp != null && p!.F != null && !string.Equals(_fp, p.F, StringComparison.Ordinal))
                throw new InvalidOperationException("Cursor filter mismatch");
            _pivot = p;
        }
        return this;
    }

    public ISeekCursorBuilder<T> WithPrev(bool enable = true)
    {
        _withPrev = enable;
        return this;
    }

    public ISeekCursorBuilder<T> Limit(int size)
    {
        _limit = Math.Clamp(size, 1, 200);
        return this;
    }

    public async Task<CursorResult<TDto>> ExecuteAsync<TDto>(
        Func<T, TDto> map,
        CancellationToken ct = default)
        where TDto : notnull
    {
        var q = _src;

        // 1) Seek predicate from pivot
        if (_pivot is { } pv)
        {
            var pivotK1 = new DateTime(pv.K1, DateTimeKind.Utc);
            var pivotK2 = pv.K2;

            // Build k1/k2 access using the same parameter (no Expression.Invoke)
            var param = Expression.Parameter(typeof(T), "e");
            var k1Body = ParameterSubst.Replace(_key1.Body, _key1.Parameters[0], param);
            var k2Body = ParameterSubst.Replace(_key2.Body, _key2.Parameters[0], param);

            var k1 = k1Body; // DateTime
            var k2 = k2Body; // int

            var k1Const = Expression.Constant(pivotK1, typeof(DateTime));
            var k2Const = Expression.Constant(pivotK2, typeof(int));

            // Older: (k1 < pivot) OR (k1 == pivot AND k2 < pivot)
            // Newer: (k1 > pivot) OR (k1 == pivot AND k2 > pivot)
            Expression BuildCmp(bool gt)
            {
                var k1Cmp = gt ? Expression.GreaterThan(k1, k1Const) : Expression.LessThan(k1, k1Const);
                var k1Eq  = Expression.Equal(k1, k1Const);
                var k2Cmp = gt ? Expression.GreaterThan(k2, k2Const) : Expression.LessThan(k2, k2Const);
                return Expression.OrElse(k1Cmp, Expression.AndAlso(k1Eq, k2Cmp));
            }

            var body = pv.D == CursorHelper.Dir.Newer ? BuildCmp(gt: true) : BuildCmp(gt: false);
            var where = Expression.Lambda<Func<T, bool>>(body, param);
            q = q.Where(where);
        }

        // 2) Canonical ORDER BY (stable)
        IOrderedQueryable<T> ordered = (_desc1, _desc2) switch
        {
            (true, true)  => q.OrderByDescending(_key1).ThenByDescending(_key2),
            (true, false) => q.OrderByDescending(_key1).ThenBy(_key2),
            (false, true)  => q.OrderBy(_key1).ThenByDescending(_key2),
            _  => q.OrderBy(_key1).ThenBy(_key2)
        };

        // 3) Page (limit+1 to detect hasMore)
        var list = await ordered.Take(_limit + 1).ToListAsync(ct);
        var kept = list.Take(_limit).ToList();
        var items = kept.Select(map).ToList();

        // 4) Cursors
        string? next = null, prev = null;
        if (kept.Count > 0)
        {
            var first = kept[0];
            var last  = kept[^1];

            var firstK1Ticks = _key1C(first).Ticks;
            var firstK2 = _key2C(first);
            var lastK1Ticks = _key1C(last).Ticks;
            var lastK2 = _key2C(last);

            var dir = _pivot?.D ?? CursorHelper.Dir.Older;

            if (list.Count > _limit)
            {
                next = CursorHelper.Token.EncodeKeys(
                    new DateTime(lastK1Ticks, DateTimeKind.Utc),
                    lastK2,
                    dir,
                    v: "k2-1",
                    f: _fp);
            }

            if (_withPrev && _pivot != null)
            {
                var opposite = _pivot.D == CursorHelper.Dir.Older ? CursorHelper.Dir.Newer : CursorHelper.Dir.Older;
                prev = CursorHelper.Token.EncodeKeys(
                    new DateTime(firstK1Ticks, DateTimeKind.Utc),
                    firstK2,
                    opposite,
                    v: "k2-1",
                    f: _fp);
            }
        }

        return new CursorResult<TDto>(
            items: items,
            nextCursor: next,
            prevCursor: prev,
            hasMore: list.Count > _limit,
            limit: _limit
        );
    }

    // Helper: replace parameter in expression tree (avoids Expression.Invoke)
    private sealed class ParameterSubst : ExpressionVisitor
    {
        private readonly ParameterExpression _from, _to;
        private ParameterSubst(ParameterExpression from, ParameterExpression to) { _from = from; _to = to; }
        protected override Expression VisitParameter(ParameterExpression node) => node == _from ? _to : base.VisitParameter(node);
        public static Expression Replace(Expression body, ParameterExpression from, ParameterExpression to)
            => new ParameterSubst(from, to).Visit(body);
    }
}
