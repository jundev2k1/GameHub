using game_x.share.ExternalApi.GameProvider.Dtos;
using game_x.share.ExternalApi.GameProvider.Dtos.Deposit;
using game_x.share.ExternalApi.GameProvider.Dtos.Login;
using game_x.share.ExternalApi.GameProvider.Dtos.Register;
using game_x.share.ExternalApi.GameProvider.Dtos.Wallet;
using game_x.share.ExternalApi.GameProvider.Dtos.Withdrawal;

namespace game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;

public interface IGameProviderService
{
    Task<LoginResponse> LoginAsync(LoginRequest data, string ip);

    Task<RegisterResponse> RegisterAsync(RegisterRequest data);

    Task<WalletResponse> GetWalletAsync(WalletRequest data);

    Task<GameDepositResponse> DepositWalletAsync(GameDepositRequest data);

    Task<GameWithdrawalResponse> WithdrawalWalletAsync(GameWithdrawalRequest data);


}
