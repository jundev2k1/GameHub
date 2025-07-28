using game_x.share.Extensions;

namespace game_x.application.Exceptions;

public sealed class NotFoundException : Exception
{
    public Enum ErrorCode { get; set; } = MessageCode.System.ResourceNotFound;
    public object? ErrorDetail { get; set; }
    
    public NotFoundException(): base(MessageCode.System.ResourceNotFound.ToMessage()) { }

    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException) { }

    public NotFoundException(string name, object key) : base($"{name} ({key}) was not found")
    {
    }
    
    public NotFoundException(Enum errorCode, object? errorDetail = null) : base(errorCode.ToMessage())
    {
        ErrorCode = errorCode;
        ErrorDetail = errorDetail;
    }

    public NotFoundException(Enum errorCode, string message, object? errorDetail = null) : base(message)
    {
        ErrorCode = errorCode;
        ErrorDetail = errorDetail;
    }
}