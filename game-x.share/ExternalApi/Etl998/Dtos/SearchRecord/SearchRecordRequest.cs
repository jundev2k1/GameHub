using System.Text.Json.Serialization;

namespace game_x.share.ExternalApi.Etl998.Dtos.SearchRecord;

public sealed class SearchRecordRequest
{
    [JsonPropertyName("account")]
    public required string Account { get; set; }
    [JsonPropertyName("pwd")]
    public required string Password { get; set; } = string.Empty;
    /// <summary>Start date, e.g. "2016-7-1".</summary>
    [JsonPropertyName("dateStart")]
    public required string DateStart { get; set; } = string.Empty;
    /// <summary>End date, e.g. "2016-7-1".</summary>
    [JsonPropertyName("dateEnd")]
    public required string DateEnd { get; set; } = string.Empty;
    /// <summary>Page number, starting from 1.</summary>
    [JsonPropertyName("pageindex")]
    public required int PageIndex { get; set; }
    /// <summary>Number of records per page.</summary>
    [JsonPropertyName("pagesize")]
    public required int PageSize { get; set; }
}