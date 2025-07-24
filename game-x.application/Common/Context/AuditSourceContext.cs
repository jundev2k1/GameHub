namespace game_x.share.Context;

public static class AuditSourceContext
{
    private static readonly AsyncLocal<AuditSource?> _current = new();

    public static AuditSource Current => _current.Value ?? AuditSource.WebApi;

    public static void Set(AuditSource source) => _current.Value = source;

    public static void Reset() => _current.Value = null;

    public static IDisposable Use(AuditSource source) => new Scope(source);

    private sealed class Scope : IDisposable
    {
        private readonly AuditSource? _previous;

        public Scope(AuditSource source)
        {
            _previous = _current.Value;
            _current.Value = source;
        }

        public void Dispose() => _current.Value = _previous;
    }
}
