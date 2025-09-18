namespace game_x.api.Dtos;

public sealed class UpdateThumbnailRequest
{
    public IFormFile Thumbnail { get; set; } = default!;
}
