namespace game_x.share.ExternalApi.Atg.Dtos;

public sealed record BaseResponse<T>
{
    public required string Status { get; set; }
    public required T Data { get; set; }
}