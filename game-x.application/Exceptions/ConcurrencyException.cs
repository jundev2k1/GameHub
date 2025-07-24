namespace game_x.application.Exceptions;
public sealed class ConcurrencyException : Exception
{
    public ConcurrencyException(string? message = null, Exception? inner = null)
        : base(message ?? "A concurrency conflict occurred.", inner) { }

    public ConcurrencyException() : base()
    {
    }

    public ConcurrencyException(string? message) : base(message)
    {
    }
}