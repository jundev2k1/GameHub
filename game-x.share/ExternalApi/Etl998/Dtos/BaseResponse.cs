using System.Text.Json;
using System.Text.Json.Serialization;

namespace game_x.share.ExternalApi.Etl998.Dtos;

public class BaseResponse
{
    /// <summary>Result object. Null when not successful</summary>
    [JsonPropertyName("result")]
    public JsonElement Result { get; set; }
    /// <summary>Error code. 0 means success.</summary>
    [JsonPropertyName("errorCode")]
    public int ErrorCode { get; set; }
    /// <summary>Detailed error message.</summary>
    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }
    public ExtendResponse? Extend { get; set; }
}

public class ExtendResponse
{
    /// <summary>Total number of records.</summary>
    [JsonPropertyName("totalcount")]
    public int? TotalCount { get; set; }
    /// <summary>Number of records per page.</summary>
    [JsonPropertyName("pagesize")]
    public int? PageSize { get; set; }
    /// <summary>Current page index.</summary>
    [JsonPropertyName("pageindex")]
    public int? PageIndex { get; set; }
    /// <summary>Total win/loss amount.</summary>
    [JsonPropertyName("totalwinlost")]
    public string? TotalWinLost { get; set; }
    /// <summary>Total rolling / rebate amount.</summary>
    [JsonPropertyName("totalximaliang")]
    public string? TotalCashback { get; set; }
}