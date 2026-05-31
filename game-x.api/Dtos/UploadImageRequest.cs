namespace game_x.api.Dtos;

public sealed class UploadImageRequest
{
    public IFormFile Image { get; set; } = default!;
}
