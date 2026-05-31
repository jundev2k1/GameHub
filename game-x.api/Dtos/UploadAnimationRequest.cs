namespace game_x.api.Dtos;

public class UploadAnimationRequest
{
    public IFormFile? Image { get; set; }
    public int Duration { get; set; } = 1000;
}
