using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace game_x.api.Middleware;

public sealed class ExceptionMiddleware(RequestDelegate next, IAppLogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
    {
        var (statusCode, code, message, systemLog, errorDetail) = MapToErrorInfo(ex);
        logger.LogError("Handled error({ErrorCode}):\n{Message}\n{ErrorDetail}", Convert.ToInt32(code), systemLog, errorDetail.ToStringOrEmpty());

        var response = ApiResponseFactory.Error(code, message, (int)statusCode, errorDetail);
        httpContext.Response.StatusCode = (int)statusCode;
        await httpContext.Response.WriteAsJsonAsync(response);
    }

    private static (HttpStatusCode StatusCode, Enum Code, string Message, string SystemLog, object? ErrorDetail) MapToErrorInfo(Exception ex)
    {
        static string CreateBadRequestMessage(IDictionary<string, string[]> messages, string message)
        {
            messages.Add("ErrorCode", [message]);
            return JsonSerializer.Serialize(messages);
        }
        return ex switch
        {
            BadRequestException badRequest =>
            (
                HttpStatusCode.BadRequest,
                badRequest.ErrorCode,
                badRequest.ErrorCode.ToMessage()!,
                CreateBadRequestMessage(badRequest.ValidationErrors, badRequest.Message),
                badRequest.ErrorDetail
            ),

            NotFoundException notFound =>
            (
                HttpStatusCode.NotFound,
                notFound.ErrorCode,
                notFound.ErrorCode.ToMessage()!,
                notFound.Message,
                notFound.ErrorDetail
            ),

            UnauthorizedException unauthorized =>
            (
                HttpStatusCode.Unauthorized,
                unauthorized.ErrorCode,
                unauthorized.ErrorCode.ToMessage()!,
                unauthorized.Message,
                unauthorized.ErrorDetail
                
            ),

            ForbiddenException forbidden =>
            (
                HttpStatusCode.Forbidden,
                forbidden.ErrorCode,
                forbidden.ErrorCode.ToMessage()!,
                forbidden.Message,
                forbidden.ErrorDetail
            ),

            ValidationException validation =>
            (
                HttpStatusCode.BadRequest,
                MessageCode.System.ValidateFailed,
                MessageCode.System.ValidateFailed.ToMessage()!,
                JsonSerializer.Serialize(validation.Errors),
                validation.ErrorDetail
            ),

            ArgumentException argument =>
            (
                HttpStatusCode.BadRequest,
                MessageCode.System.InvalidParameters,
                MessageCode.System.InvalidParameters.ToMessage()!,
                argument.Message,
                null
            ),

            InvalidArgumentException invalidArgument =>
            (
                HttpStatusCode.BadRequest,
                invalidArgument.ErrorCode,
                invalidArgument.ErrorCode.ToMessage()!,
                invalidArgument.Message,
                invalidArgument.ErrorDetail
            ),

            ExternalServiceException external =>
            (
                HttpStatusCode.BadRequest,
                external.ErrorCode,
                external.ErrorCode.ToMessage()!,
                external.Message,
                external.ErrorDetail
            ),

            TimeoutException timeout =>
            (
                HttpStatusCode.RequestTimeout,
                MessageCode.System.RequestTimeout,
                MessageCode.System.RequestTimeout.ToMessage()!,
                timeout.Message,
                null
            ),

            OperationCanceledException canceled =>
            (
                HttpStatusCode.RequestTimeout,
                MessageCode.System.RequestCancelled,
                MessageCode.System.RequestCancelled.ToMessage()!,
                canceled.Message,
                null
            ),

            DbUpdateException db =>
            (
                HttpStatusCode.Conflict,
                MessageCode.System.Conflict,
                MessageCode.System.Conflict.ToMessage()!,
                db.Message,
                null
            ),

            ConcurrencyException db =>
            (
                HttpStatusCode.Conflict,
                MessageCode.System.Conflict,
                MessageCode.System.Conflict.ToMessage()!,
                db.Message,
                db.ErrorDetail
            ),

            _ =>
            (
                HttpStatusCode.InternalServerError,
                MessageCode.System.SystemError,
                MessageCode.System.SystemError.ToMessage()!,
                JsonSerializer.Serialize(ex.StackTrace),
                new { ex.Message }
            )
        };
    }
}