using game_x.share.ExternalApi.GameProvider.Dtos.Deposit;
using game_x.share.ExternalApi.GameProvider.Dtos.Login;
using game_x.share.ExternalApi.GameProvider.Dtos.Logout;
using game_x.share.ExternalApi.GameProvider.Dtos.Register;
using game_x.share.ExternalApi.GameProvider.Dtos.Report;
using game_x.share.ExternalApi.GameProvider.Dtos.Wallet;
using game_x.share.ExternalApi.GameProvider.Dtos.Withdrawal;

namespace game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;

public interface IGameProviderService
{
    Task<GameLoginResponse> LoginAsync(GameLoginRequest data, string ip);

    Task<GameLogoutResponse> LogoutAsync(GameLogoutRequest data);

    Task<GameRegisterResponse> RegisterAsync(GameRegisterRequest data);

    Task<GameWalletResponse> GetWalletAsync(GameWalletRequest data);

    Task<GameDepositResponse> DepositWalletAsync(GameDepositRequest data);

    Task<GameWithdrawalResponse> WithdrawalWalletAsync(GameWithdrawalRequest data);

    Task<GameReportResponse> GetReportAsync(GameReportRequest data, string account);
}
