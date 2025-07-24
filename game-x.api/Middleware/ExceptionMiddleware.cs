using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace game_x.api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, IAppLogger<ExceptionMiddleware> logger)
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
        var (statusCode, code, message, systemLog) = MapToErrorInfo(ex);
        logger.LogError("Handled error({ErrorCode}):\n{Message}", Convert.ToInt32(code), systemLog);

        var response = ApiResponseFactory.Error(code, message, (int)statusCode);
        httpContext.Response.StatusCode = (int)statusCode;
        await httpContext.Response.WriteAsJsonAsync(response);
    }

    private static (HttpStatusCode StatusCode, Enum Code, string Message, string SystemLog) MapToErrorInfo(Exception ex)
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
                CreateBadRequestMessage(badRequest.ValidationErrors, badRequest.Message)
            ),

            NotFoundException notFound =>
            (
                HttpStatusCode.NotFound,
                notFound.ErrorCode,
                notFound.ErrorCode.ToMessage()!,
                notFound.Message
            ),

            UnauthorizedException unauthorized =>
            (
                HttpStatusCode.Unauthorized,
                unauthorized.ErrorCode,
                unauthorized.ErrorCode.ToMessage()!,
                unauthorized.Message
            ),

            ForbiddenException forbidden =>
            (
                HttpStatusCode.Forbidden,
                forbidden.ErrorCode,
                forbidden.ErrorCode.ToMessage()!,
                forbidden.Message
            ),

            ValidationException validation =>
            (
                HttpStatusCode.BadRequest,
                MessageCode.System.ValidateFailed,
                MessageCode.System.ValidateFailed.ToMessage()!,
                JsonSerializer.Serialize(validation.Errors)
            ),

            ArgumentException argument =>
            (
                HttpStatusCode.BadRequest,
                MessageCode.System.InvalidParameters,
                MessageCode.System.InvalidParameters.ToMessage()!,
                argument.Message
            ),

            InvalidArgumentException invalidArgument =>
            (
                HttpStatusCode.BadRequest,
                invalidArgument.ErrorCode,
                invalidArgument.ErrorCode.ToMessage()!,
                invalidArgument.Message
            ),

            ExternalServiceException external =>
            (
                HttpStatusCode.BadRequest,
                external.ErrorCode,
                external.ErrorCode.ToMessage()!,
                external.Message
            ),

            TimeoutException timeout =>
            (
                HttpStatusCode.RequestTimeout,
                MessageCode.System.RequestTimeout,
                MessageCode.System.RequestTimeout.ToMessage()!,
                timeout.Message
            ),

            OperationCanceledException canceled =>
            (
                HttpStatusCode.RequestTimeout,
                MessageCode.System.RequestCancelled,
                MessageCode.System.RequestCancelled.ToMessage()!,
                canceled.Message
            ),

            DbUpdateException db =>
            (
                HttpStatusCode.Conflict,
                MessageCode.System.Conflict,
                MessageCode.System.Conflict.ToMessage()!,
                db.Message
            ),

            ConcurrencyException db =>
            (
                HttpStatusCode.Conflict,
                MessageCode.System.Conflict,
                MessageCode.System.Conflict.ToMessage()!,
                db.Message
            ),

            _ =>
            (
                HttpStatusCode.InternalServerError,
                MessageCode.System.SystemError,
                MessageCode.System.SystemError.ToMessage()!,
                JsonSerializer.Serialize(ex.StackTrace)
            )
        };
    }
}
