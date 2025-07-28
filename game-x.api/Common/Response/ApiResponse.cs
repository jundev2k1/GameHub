namespace game_x.api.Common.Response;

public sealed class ApiResponse<T>
{
    /// <summary>The data returned in the response, if any.</summary>
    public T? Data { get; set; }
    /// <summary>Indicates whether the request was successful.</summary>
    public bool Success { get; set; } = true;
    /// <summary>The status code of the response.</summary>
    public int StatusCode { get; set; }
    /// <summary>A message codes make it easier for the FE team to translate messages returned from the server.</summary>
    public int? MessageCode { get; set; }
    /// <summary>A message providing additional information about the response.</summary>
    public string? Message { get; set; }
    public object? ErrorDetail { get; set; }
}