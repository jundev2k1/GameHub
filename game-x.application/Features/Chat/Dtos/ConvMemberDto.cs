namespace game_x.application.Features.Chat.Dtos;

public record ConvMemberDto
{
    public string UserId { get; set; } = string.Empty;
    public bool? IsHidden { get; set; }
}