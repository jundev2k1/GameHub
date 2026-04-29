namespace game_x.application.Contract.Infrastructure.Caching;

public interface IAsymmetricKeyCacheService
{
    void Refresh();

    string GameXPrivateKey { get; }

    string GameXPublicKey { get; }

    string UxmPublicKey { get; }

    string FastPayPublicKey { get; }

    string SlotPublicKey { get; }
}