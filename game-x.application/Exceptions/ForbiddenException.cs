using game_x.share.Extensions;

namespace game_x.application.Exceptions;

public sealed class ForbiddenException : Exception
{
    public Enum ErrorCode { get; set; } = MessageCode.System.Forbidden;
    public object? ErrorDetail { get; set; }

    public ForbiddenException() : base("Forbidden") { }

    public ForbiddenException(string message) : base(message) { }

    public ForbiddenException(string message, Exception innerException)
        : base(message, innerException) { }

    public ForbiddenException(Enum messageCode, object? errorDetail = null) : base(messageCode.ToMessage())
    {
        ErrorCode = messageCode;
        ErrorDetail =  errorDetail;
    }

    public ForbiddenException(Enum messageCode, string message, object? errorDetail = null) : base(message)
    {
        ErrorCode = messageCode;
        ErrorDetail = errorDetail;
    }
}