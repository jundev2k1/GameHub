using game_x.share.Extensions;

namespace game_x.application.Exceptions;

public sealed class ForbiddenException : Exception
{
    public Enum ErrorCode { get; set; } = MessageCode.System.Forbidden;

    public ForbiddenException() : base("Forbidden") { }

    public ForbiddenException(string message) : base(message) { }

    public ForbiddenException(string message, Exception innerException)
        : base(message, innerException) { }

    public ForbiddenException(Enum messageCode) : base(messageCode.ToMessage())
    {
        ErrorCode = messageCode;
    }

    public ForbiddenException(Enum messageCode, string message) : base(message)
    {
        ErrorCode = messageCode;
    }
}