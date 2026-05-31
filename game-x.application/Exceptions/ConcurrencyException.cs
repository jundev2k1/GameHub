namespace game_x.application.Exceptions;
public sealed class ConcurrencyException : Exception
{
    public object? ErrorDetail { get; set; }
    public ConcurrencyException(string? message = null, Exception? inner = null)
        : base(message ?? "A concurrency conflict occurred.", inner) { }

    public ConcurrencyException() : base()
    {
    }

    public ConcurrencyException(string? message, object? errorDetail = null) : base(message)
    {
        ErrorDetail = errorDetail;
    }
}