namespace game_x.share.ExternalApi.Atg.Dtos.GameProvider;

public class GameProviderResponse
{
    public required ICollection<GameProviderItem> GameProviders { get; set; }
    public required MetaResponse Meta { get; set; }
}

public sealed record GameProviderItem(int Id, string Name, string Code);