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
    public static ApiResponse<object?> Error(Enum code, string? message = null, int? statusCode = (int)HttpStatusCode.BadRequest)
    {
        var response = new ApiResponse<object?>
        {
            Data = null,
            Success = false,
            MessageCode = Convert.ToInt32(code),
            Message = message ?? code.ToMessage(),
            StatusCode = statusCode ?? code.ToHttpStatus()
        };
        return response;
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
            StatusCode = statusCode
        };

        return new ObjectResult(response) { StatusCode = statusCode };
    }
}
