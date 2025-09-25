namespace game_x.api.Dtos;

public sealed class UpdateImageRequest
{
    public IFormFile Image { get; set; } = default!;
}
