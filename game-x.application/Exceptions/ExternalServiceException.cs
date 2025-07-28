namespace game_x.application.Exceptions;

public sealed class ExternalServiceException : Exception
{
    public Enum ErrorCode { get; set; } = MessageCode.System.DependencyFailure;
    public object? ErrorDetail { get; set; }

    public ExternalServiceException()
        : base("External service error.") { }

    public ExternalServiceException(string message)
        : base(message) { }

    public ExternalServiceException(Enum? errorCode, object? errorDetail = null)
    {
        ErrorCode = errorCode ?? MessageCode.System.DependencyFailure;
        ErrorDetail = errorDetail;
    }

    public ExternalServiceException(string message, Enum? errorCode = null, object? errorDetail = null)
        : base(message)
    {
        ErrorCode = errorCode ?? MessageCode.System.DependencyFailure;
        ErrorDetail = errorDetail;
    }

    public ExternalServiceException(string message, Exception innerException)
        : base(message, innerException) { }
}
