using game_x.share.ExternalApi.Atg.Enums;

namespace game_x.share.ExternalApi.Atg.Dtos.GetGames;

public class GetGamesResponse
{
    public required ICollection<GameItem> Games { get; set; }
    public required MetaResponse Meta { get; set; }
}

public sealed record GameItem(
    int Id,
    int GameId,
    GameType GameType,
    string Category,
    string Name, 
    int ProviderId,
    string Code,
    bool Actived,
    IReadOnlyCollection<string> Locales,
    int Disabled,
    bool IsNew,
    bool IsHot,
    string Url);