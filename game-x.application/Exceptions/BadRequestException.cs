using FluentValidation.Results;
using game_x.share.Extensions;

namespace game_x.application.Exceptions;

public sealed class BadRequestException : Exception
{
    public IDictionary<string, string[]> ValidationErrors { get; set; } = new Dictionary<string, string[]>();

    public Enum ErrorCode { get; set; } = MessageCode.System.ValidateFailed;

    public BadRequestException() { }

    public BadRequestException(string message) : base(message) { }

    public BadRequestException(string message, Exception innerException)
        : base(message, innerException) { }

    public BadRequestException(string message, ValidationResult validationResult, Enum errorCode) : base(message)
    {
        ValidationErrors = validationResult.ToDictionary();
        ErrorCode = errorCode;
    }

    public BadRequestException(Enum errorCode) : base(errorCode.ToMessage())
    {
        ErrorCode = errorCode;
    }

    public BadRequestException(Enum errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }
}
