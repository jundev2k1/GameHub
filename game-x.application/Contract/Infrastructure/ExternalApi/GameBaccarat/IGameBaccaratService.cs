using game_x.share.ExternalApi.GameBaccarat.Dtos.Login;
using game_x.share.ExternalApi.GameBaccarat.Dtos.Register;

namespace game_x.application.Contract.Infrastructure.ExternalApi.GameBaccarat;

public interface IGameBaccaratService
{
    Task<GameBaccaratLoginResponse> LoginAsync(GameBaccaratLoginRequest request);

    Task<GameBaccaratRegisterResponse> RegisterAsync(GameBaccaratRegisterRequest request);
}
