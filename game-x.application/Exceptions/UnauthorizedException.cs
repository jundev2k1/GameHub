using game_x.share.Extensions;

namespace game_x.application.Exceptions;

public sealed class UnauthorizedException : Exception
{
    public Enum ErrorCode { get; set; } = MessageCode.System.Unauthorized;
    public object? ErrorDetail { get; set; }

    public UnauthorizedException() : base("Unauthorized") { }

    public UnauthorizedException(string message) : base(message) { }

    public UnauthorizedException(Enum errorCode, object? errorDetail = null) : base(errorCode.ToMessage())
    {
        ErrorCode = errorCode;
        ErrorDetail = errorDetail;
    }

    public UnauthorizedException(string message, Exception innerException)
        : base(message, innerException) { }
}
