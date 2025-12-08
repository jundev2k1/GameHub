using game_x.share.ExternalApi.GameBaccarat.Dtos.Deposit;
using game_x.share.ExternalApi.GameBaccarat.Dtos.GetWallet;
using game_x.share.ExternalApi.GameBaccarat.Dtos.Login;
using game_x.share.ExternalApi.GameBaccarat.Dtos.Register;

namespace game_x.application.Contract.Infrastructure.ExternalApi.GameBaccarat;

public interface IGameBaccaratService
{
    Task<GameBaccaratLoginResponse> LoginAsync(GameBaccaratLoginRequest request);

    Task<GameBaccaratRegisterResponse> RegisterAsync(GameBaccaratRegisterRequest request);

    Task<GameBaccaratGetWalletResponse> GetWalletAsync(GameBaccaratGetWalletRequest request);

    Task DepositAsync(GameBaccaratDepositRequest request);
}
