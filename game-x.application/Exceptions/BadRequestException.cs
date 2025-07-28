using FluentValidation.Results;
using game_x.share.Extensions;

namespace game_x.application.Exceptions;

public sealed class BadRequestException : Exception
{
    public IDictionary<string, string[]> ValidationErrors { get; set; } = new Dictionary<string, string[]>();

    public Enum ErrorCode { get; set; } = MessageCode.System.ValidateFailed;
    public object? ErrorDetail { get; set; }

    public BadRequestException() { }

    public BadRequestException(string message) : base(message) { }

    public BadRequestException(string message, Exception innerException)
        : base(message, innerException) { }

    public BadRequestException(string message, ValidationResult validationResult, Enum errorCode, object? errorDetail = null) : base(message)
    {
        ValidationErrors = validationResult.ToDictionary();
        ErrorCode = errorCode;
        ErrorDetail = errorDetail;
    }

    public BadRequestException(Enum errorCode, object? errorDetail = null) : base(errorCode.ToMessage())
    {
        ErrorCode = errorCode;
        ErrorDetail = errorDetail;
    }

    public BadRequestException(Enum errorCode, string message, object? errorDetail = null) : base(message)
    {
        ErrorCode = errorCode;
        ErrorDetail = errorDetail;
    }
}
