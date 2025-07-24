namespace game_x.application.Contract.Infrastructure.Caching;

public interface IAsymmetricKeyCacheService
{
    void Refresh();
    string GalaxyPrivateKey { get; }
    string GalaxyPublicKey { get; }
    string UxmPublicKey { get; }
}