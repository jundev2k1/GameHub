namespace game_x.application.Exceptions;

public sealed class InvalidArgumentException : Exception
{
    public Enum ErrorCode { get; set; } = MessageCode.System.InvalidParameters;

    public InvalidArgumentException()
        : base("Invalid argument provided.") { }

    public InvalidArgumentException(string message)
        : base(message) { }

    public InvalidArgumentException(Enum? errorCode)
    {
        ErrorCode = errorCode ?? MessageCode.System.InvalidParameters;
    }

    public InvalidArgumentException(string message, Enum? errorCode = null)
        : base(message)
    {
        ErrorCode = errorCode ?? MessageCode.System.InvalidParameters;
    }

    public InvalidArgumentException(string message, Exception innerException)
        : base(message, innerException) { }

    public InvalidArgumentException(string name, object key) : base($"{name} ({key}) is invalid value.")
    {
    }
}
