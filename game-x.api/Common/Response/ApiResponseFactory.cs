using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace game_x.api.Common.Response;

public static class ApiResponseFactory
{
    // ------ SUCCESS -------
    public static IActionResult Ok<T>(T data, Enum? code = null)
        => Build(data, code ?? MessageCode.System.Success, (int)HttpStatusCode.OK);

    public static IActionResult Created<T>(T data, Enum? code = null)
        => Build(data, code ?? MessageCode.System.Created, (int)HttpStatusCode.Created);
    public static IActionResult Created(Enum? code = null)
        => Build<object?>(null, code ?? MessageCode.System.Created, (int)HttpStatusCode.Created);

    public static IActionResult NoContent(Enum? code = null)
        => Build<object?>(null, code ?? MessageCode.System.NoContent, (int)HttpStatusCode.OK);

    // ------ ERROR --------
    public static ApiResponse<object?> Error(Enum code, string? message = null, int? statusCode = (int)HttpStatusCode.BadRequest, object? errorDetail = null)
    {
        var response = new ApiResponse<object?>
        {
            Data = null,
            Success = false,
            MessageCode = Convert.ToInt32(code),
            Message = message ?? code.ToMessage(),
            StatusCode = statusCode ?? code.ToHttpStatus(),
            ErrorDetail = errorDetail
        };
        return response;
    }

    public static IActionResult BadRequest(Enum code, string? message = null, object? errorDetail = null)
    {
        var response = new ApiResponse<object?>
        {
            Data = null,
            Success = false,
            MessageCode = Convert.ToInt32(code),
            Message = message ?? code.ToMessage(),
            StatusCode = (int)HttpStatusCode.BadRequest,
            ErrorDetail = errorDetail
        };
        return new ObjectResult(response) { StatusCode = (int)HttpStatusCode.BadRequest };
    }

    public static IActionResult Forbidden(Enum code, string? message = null, object? errorDetail = null)
    {
        var response = new ApiResponse<object?>
        {
            Data = null,
            Success = false,
            MessageCode = Convert.ToInt32(code),
            Message = message ?? code.ToMessage(),
            StatusCode = (int)HttpStatusCode.Forbidden,
            ErrorDetail = errorDetail
        };
        return new ObjectResult(response) { StatusCode = (int)HttpStatusCode.BadRequest };
    }

    // ------ GENERIC BUILD -------
    private static IActionResult Build<T>(T data, Enum code, int statusCode)
    {
        var response = new ApiResponse<T>
        {
            Data = data,
            Success = true,
            MessageCode = Convert.ToInt32(code),
            Message = code.ToMessage(),
            StatusCode = statusCode,
            ErrorDetail = null
        };

        return new ObjectResult(response) { StatusCode = statusCode };
    }
}
