using game_x.share.Extensions;

namespace game_x.application.Exceptions;

public sealed class NotFoundException : Exception
{
    public Enum ErrorCode { get; set; } = MessageCode.System.ResourceNotFound;
    
    public NotFoundException(): base(MessageCode.System.ResourceNotFound.ToMessage()) { }

    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException) { }

    public NotFoundException(string name, object key) : base($"{name} ({key}) was not found")
    {
    }
    
    public NotFoundException(Enum errorCode) : base(errorCode.ToMessage())
    {
        ErrorCode = errorCode;
    }

    public NotFoundException(Enum errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }
}