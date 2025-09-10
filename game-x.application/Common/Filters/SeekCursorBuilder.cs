using System.Linq.Expressions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.share.Helper;
using Microsoft.EntityFrameworkCore;

namespace game_x.application.Common.Filters;

/// <summary>
/// <para>Seek-based paginator over an IQueryable using two keys:</para>
/// <list type="bullet">
///   <item><description>key1: DateTime (primary)</description></item>
///   <item><description>key2: int (tiebreaker)</description></item>
/// </list>
/// <para>Fixed cursor semantics:</para>
/// <list type="bullet">
///   <item><description><b>nextCursor</b> → Newer (towards newest)</description></item>
///   <item><description><b>prevCursor</b> → Older (towards oldest)</description></item>
/// </list>
/// <para>Works with ASC/DESC by inverting ORDER BY when needed and reversing the list.</para>
/// </summary>
public sealed class SeekCursorBuilder<T> : ISeekCursorBuilder<T> where T : notnull
{
    // ---------- Fields (configuration/state) ----------
    /// <summary>Source query to paginate.</summary>
    private readonly IQueryable<T> _src;

    /// <summary>Primary sort key (DateTime).</summary>
    private Expression<Func<T, DateTime>> _key1 = null!;

    /// <summary>Secondary sort key (int).</summary>
    private Expression<Func<T, int>> _key2 = null!;

    /// <summary>Sort directions for key1/key2 (true=DESC, false=ASC).</summary>
    private bool _desc1, _desc2;

    /// <summary>Decoded pivot from cursor (where to continue from).</summary>
    private CursorHelper.Payload? _pivot;
    
    private int _limit = 20;

    /// <summary>Compiled getters for edge extraction after materialization.</summary>
    private Func<T, DateTime> _key1C = null!;
    private Func<T, int> _key2C = null!;

    /// <summary>Optional filter fingerprint to validate cursor affinity.</summary>
    private string? _fp;

    // Small immutable container for edge keys
    private readonly record struct EdgeKeys(long K1Ticks, int K2);

    private SeekCursorBuilder(IQueryable<T> src) { _src = src; }
    public static SeekCursorBuilder<T> For(IQueryable<T> src) => new(src);

    /// <summary>
    /// Configure the two sort keys (key1: DateTime, key2: int).
    /// Also compiles delegates to extract keys from materialized entities.
    /// </summary>
    public ISeekCursorBuilder<T> Keys(Expression<Func<T, DateTime>> key1, Expression<Func<T, int>> key2)
    {
        _key1 = key1;
        _key2 = key2;
        _key1C = key1.Compile();
        _key2C = key2.Compile();
        return this;
    }

    /// <summary>Set sort directions: desc1 for key1, desc2 for key2.</summary>
    public ISeekCursorBuilder<T> Sort(bool desc1, bool desc2)
    {
        _desc1 = desc1;
        _desc2 = desc2;
        return this;
    }
    
    public ISeekCursorBuilder<T> Limit(int size)
    {
        _limit = Math.Clamp(size, 1, 200);
        return this;
    }

    /// <summary>
    /// Provide an incoming cursor.
    /// If valid, it becomes the pivot to seek from.
    /// Optional fingerprint (fp) is compared to ensure the cursor belongs to this filter/sort.
    /// </summary>
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

    /// <summary>
    /// Execute seek pagination with fixed cursor semantics:
    ///   - nextCursor (Dir.Newer) from the newer edge
    ///   - prevCursor (Dir.Older) from the older edge
    /// </summary>
    public async Task<CursorResult<TDto>> ExecuteAsync<TDto>(
    Func<T, TDto> map,
    CancellationToken ct = default)
    where TDto : notnull
{
    EnsureConfigured();

    // (1) Apply pivot predicate if present
    var q = ApplyPivot(_src);

    // (2) Choose ordering (invert when seek a direction opposes a UI direction)
    var (ordered, invertOrder) = ApplyOrdering(q);

    // (3) Fetch (limit+1). IMPORTANT: slice OUT the extra before any reversing.
    var raw = await ordered.Take(_limit + 1).ToListAsync(ct);
    bool hasExtra = raw.Count > _limit;

    List<T> kept;
    if (invertOrder)
    {
        // Query order is opposite to UI.
        // Extra is at the tail in query order -> drop it first, then restore UI order.
        kept = (hasExtra ? raw.Take(_limit) : raw).ToList(); // drop extra
        kept.Reverse(); // back to UI order
    }
    else
    {
        // Query order matches UI; extra is at the tail -> just drop it.
        kept = raw.Take(_limit).ToList();
    }

    // (4) Map
    var items = kept.Select(map).ToList();
    items.Reverse();
    
    // (5) Build cursors
    string? next = null, prev = null;
    if (kept.Count > 0)
    {
        // Select page edges (in UI order)
        var (edgeNewer, edgeOlder) = SelectEdges(kept);

        // Determine which side the extra item belongs to
        var (hasNewer, hasOlder) = DetectSides(
            hasPivot: _pivot != null,
            pivotDir: _pivot?.D, // if came via Older, always allow going back Newer
            invertOrder: invertOrder,
            uiDesc: _desc1,
            hasExtra: hasExtra
        );

        if (hasNewer)
            next = BuildCursor(edgeNewer, CursorHelper.Dir.Newer);

        if (hasOlder)
            prev = BuildCursor(edgeOlder, CursorHelper.Dir.Older);
    }

    return new CursorResult<TDto>(
        items: items,
        nextCursor: next,
        prevCursor: prev,
        limit: _limit
    );
}
    
    // --------------- Helpers ---------------

    /// <summary>Ensure Keys() was called.</summary>
    private void EnsureConfigured()
    {
        if (_key1 is null || _key2 is null || _key1C is null || _key2C is null)
            throw new InvalidOperationException("SeekCursorBuilder: Keys(key1,key2) must be configured before ExecuteAsync.");
    }

    /// <summary>
    /// If a pivot is present, apply the seek predicate:
    /// <para><b>Newer</b>: <c>(k1 &gt; pv.k1) || (k1 == pv.k1 &amp;&amp; k2 &gt; pv.k2)</c></para>
    /// <para><b>Older</b>: <c>(k1 &lt; pv.k1) || (k1 == pv.k1 &amp;&amp; k2 &lt; pv.k2)</c></para>
    /// </summary>
    private IQueryable<T> ApplyPivot(IQueryable<T> src)
    {
        if (_pivot is not { } pv) return src;

        var pivotK1 = new DateTime(pv.K1, DateTimeKind.Utc);
        var pivotK2 = pv.K2;

        var param  = Expression.Parameter(typeof(T), "e");
        var k1Body = ParameterSubst.Replace(_key1.Body, _key1.Parameters[0], param);
        var k2Body = ParameterSubst.Replace(_key2.Body, _key2.Parameters[0], param);

        var k1Const = Expression.Constant(pivotK1, typeof(DateTime));
        var k2Const = Expression.Constant(pivotK2, typeof(int));

        Expression BuildCmp(bool gt)
        {
            var k1Cmp = gt ? Expression.GreaterThan(k1Body, k1Const) : Expression.LessThan(k1Body, k1Const);
            var k1Eq  = Expression.Equal(k1Body, k1Const);
            var k2Cmp = gt ? Expression.GreaterThan(k2Body, k2Const) : Expression.LessThan(k2Body, k2Const);
            return Expression.OrElse(k1Cmp, Expression.AndAlso(k1Eq, k2Cmp));
        }

        var body = pv.D == CursorHelper.Dir.Newer ? BuildCmp(gt: true) : BuildCmp(gt: false);
        var where = Expression.Lambda<Func<T, bool>>(body, param);
        return src.Where(where);
    }

    /// <summary>
    /// Build ORDER BY for the UI and (optionally) an inverted ORDER BY when seek direction
    /// conflicts with the UI direction on key1.
    /// <para>UI DESC &amp; seek Newer → invert</para>
    /// <para>UI ASC &amp; seek Older → invert</para>
    /// <para>Returns: (orderedQueryable, invertOrderFlag).</para>
    /// </summary>
    private (IOrderedQueryable<T> ordered, bool invertOrder) ApplyOrdering(IQueryable<T> q)
    {
        bool hasPivot = _pivot != null;
        bool seekingNewer = _pivot?.D == CursorHelper.Dir.Newer;

        bool invertOrder =
            hasPivot &&
            (
                (_desc1 && seekingNewer) // UI DESC + Newer
             || (!_desc1 && !seekingNewer) // UI ASC + Older
            );

        IOrderedQueryable<T> orderUi = (_desc1, _desc2) switch
        {
            (true, true) => q.OrderByDescending(_key1).ThenByDescending(_key2),
            (true, false) => q.OrderByDescending(_key1).ThenBy(_key2),
            (false, true) => q.OrderBy(_key1).ThenByDescending(_key2),
            _ => q.OrderBy(_key1).ThenBy(_key2)
        };

        IOrderedQueryable<T> orderInverted = (_desc1, _desc2) switch
        {
            (true, true) => q.OrderBy(_key1).ThenBy(_key2),
            (true, false) => q.OrderBy(_key1).ThenByDescending(_key2),
            (false, true) => q.OrderByDescending(_key1).ThenBy(_key2),
            _  => q.OrderByDescending(_key1).ThenByDescending(_key2)
        };

        return (invertOrder ? orderInverted : orderUi, invertOrder);
    }

    /// <summary>
    /// In UI DESC: first=newer, last=older.
    /// In UI ASC: last=newer, first=older.
    /// Returns the two-edge key pairs.
    /// </summary>
    private (EdgeKeys newer, EdgeKeys older) SelectEdges(List<T> kept)
    {
        var first = kept[0];
        var last = kept[^1];

        var edgeForNewer = _desc1 ? first : last;
        var edgeForOlder = _desc1 ? last : first;

        return (
            new EdgeKeys(_key1C(edgeForNewer).Ticks, _key2C(edgeForNewer)),
            new EdgeKeys(_key1C(edgeForOlder).Ticks, _key2C(edgeForOlder))
        );
    }

    /// <summary>
    /// Decide whether the "extra" item lies on the Newer side or Older side.
    /// Also: if we came via Dir.Older (prev), we ALWAYS allow going back to Newer (expose next).
    /// </summary>
    private static (bool hasNewer, bool hasOlder) DetectSides(
        bool hasPivot,
        CursorHelper.Dir? pivotDir,
        bool invertOrder,
        bool uiDesc,
        bool hasExtra)
    {
        // If there is NO extra item on the queried side:
        // still allow going back toward the side that we came from.
        if (!hasExtra)
        {
            bool newer = hasPivot && pivotDir == CursorHelper.Dir.Older; // came via Older -> can go back Newer
            bool older = hasPivot && pivotDir == CursorHelper.Dir.Newer; // came via Newer -> can go back Older
            return (newer, older);
        }

        // There IS an extra item on whichever side the current ordered query points to.
        (bool hasNewer, bool hasOlder) result;

        if (!hasPivot)
        {
            // First landing page (no cursor yet)
            result = uiDesc ? (false, true) : (true, false);
        }
        else if (!invertOrder)
        {
            // Extra lies on the UI-forward side
            result = uiDesc ? (false, true) : (true, false);
        }
        else
        {
            // Extra lies on the UI-backward side
            result = uiDesc ? (true, false) : (false, true);
        }

        // Symmetric guarantee: always allow returning to the opposite side we came from.
        if (hasPivot && pivotDir == CursorHelper.Dir.Older) result.hasNewer = true;
        if (hasPivot && pivotDir == CursorHelper.Dir.Newer) result.hasOlder = true;

        return result;
    }


    /// <summary>Encode a cursor from edge keys and a direction (Newer/Older), including fingerprint.</summary>
    private string BuildCursor(EdgeKeys edge, CursorHelper.Dir dir)
    {
        return CursorHelper.Token.EncodeKeys(
            new DateTime(edge.K1Ticks, DateTimeKind.Utc),
            edge.K2,
            dir,
            v: "k2-1",
            f: _fp
        );
    }

    /// <summary>
    /// Expression visitor to rebind key expressions to a shared parameter (EF-friendly, no Invoke).
    /// </summary>
    private sealed class ParameterSubst : ExpressionVisitor
    {
        private readonly ParameterExpression _from, _to;
        private ParameterSubst(ParameterExpression from, ParameterExpression to) { _from = from; _to = to; }

        protected override Expression VisitParameter(ParameterExpression node)
            => node == _from ? _to : base.VisitParameter(node);

        public static Expression Replace(Expression body, ParameterExpression from, ParameterExpression to)
            => new ParameterSubst(from, to).Visit(body);
    }
}
