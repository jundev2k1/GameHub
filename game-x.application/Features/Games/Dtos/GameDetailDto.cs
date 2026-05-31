namespace game_x.application.Features.Games.Dtos;

public sealed class GameDetailDto : GameInfoDto
{
    public GameTranslationInfo[] Translations => [.. GameTranslations.Values];
}
