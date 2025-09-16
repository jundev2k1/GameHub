using System.Text.Json.Serialization;

namespace game_x.api.Common;

public class SrsEventHookRequest
{
    [JsonPropertyName("server_id")]
    public string ServerId { get; set; } = string.Empty;

    [JsonPropertyName("service_id")]
    public string ServiceId { get; set; } = string.Empty;

    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty;

    [JsonPropertyName("client_id")]
    public string ClientId { get; set; } = string.Empty;

    [JsonPropertyName("ip")]
    public string Ip { get; set; } = string.Empty;

    [JsonPropertyName("vhost")]
    public string Vhost { get; set; } = string.Empty;

    [JsonPropertyName("app")]
    public string App { get; set; } = string.Empty;

    [JsonPropertyName("stream")]
    public string Stream { get; set; } = string.Empty;

    [JsonPropertyName("param")]
    public string Param { get; set; } = string.Empty;

    [JsonPropertyName("tcUrl")]
    public string TcUrl { get; set; } = string.Empty;

    [JsonPropertyName("pageUrl")]
    public string PageUrl { get; set; } = string.Empty;
}
