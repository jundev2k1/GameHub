namespace game_x.application.Exceptions;

public sealed class ExternalServiceException : Exception
{
    public Enum ErrorCode { get; set; } = MessageCode.System.DependencyFailure;

    public ExternalServiceException()
        : base("External service error.") { }

    public ExternalServiceException(string message)
        : base(message) { }

    public ExternalServiceException(Enum? errorCode)
    {
        ErrorCode = errorCode ?? MessageCode.System.DependencyFailure;
    }

    public ExternalServiceException(string message, Enum? errorCode = null)
        : base(message)
    {
        ErrorCode = errorCode ?? MessageCode.System.DependencyFailure;
    }

    public ExternalServiceException(string message, Exception innerException)
        : base(message, innerException) { }
}
