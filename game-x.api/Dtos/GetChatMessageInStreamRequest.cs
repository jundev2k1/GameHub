namespace game_x.api.Dtos;

public sealed class GetChatMessageInStreamRequest
{
    public Guid MessageId { get; set; }
    public bool IsNext { get; set; } = false;
    public int PageSize { get; set; } = 20;
}
