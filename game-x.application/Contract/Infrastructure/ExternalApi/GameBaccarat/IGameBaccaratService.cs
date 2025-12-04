using game_x.share.ExternalApi.GameBaccarat.Dtos.Login;

namespace game_x.application.Contract.Infrastructure.ExternalApi.GameBaccarat;

public interface IGameBaccaratService
{
    Task<GameBaccaratLoginResponse> LoginAsync(GameBaccaratLoginRequest request);
}
