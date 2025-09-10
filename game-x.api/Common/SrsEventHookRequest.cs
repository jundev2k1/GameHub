namespace game_x.api.Common;

public class SrsEventHookRequest
{
    public string Action { get; set; } = string.Empty;
    public required string ClientId { get; set; }
    public required string Ip { get; set; }
    public required string Vhost { get; set; }
    public required string App { get; set; }
    public required string Stream { get; set; }
    public required string Param { get; set; }
    public required string ServerId { get; set; }
    public required string StreamUrl { get; set; }
    public required string StreamId { get; set; }
}
