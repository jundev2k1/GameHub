namespace game_x.domain.Exceptions;

public abstract class DomainException : Exception
{
    public virtual System.Enum ErrorCode { get; set; } = MessageCode.System.InvalidOperation;

    public DomainException(string? message)
        : base(message)
    {
    }

    public DomainException(string? message, Exception? innerError)
        : base(message, innerError)
    {
    }
}
