using game_x.share.Extensions;

namespace game_x.application.Exceptions;

public sealed class UnauthorizedException : Exception
{
    public Enum ErrorCode { get; set; } = MessageCode.System.Unauthorized;

    public UnauthorizedException() : base("Unauthorized") { }

    public UnauthorizedException(string message) : base(message) { }

    public UnauthorizedException(Enum errorCode) : base(errorCode.ToMessage())
    {
        ErrorCode = errorCode;
    }

    public UnauthorizedException(string message, Exception innerException)
        : base(message, innerException) { }
}
