namespace game_x.api.Dtos;

public sealed class UpdateImageRequest
{
    public IFormFile Thumbnail { get; set; } = default!;
}
