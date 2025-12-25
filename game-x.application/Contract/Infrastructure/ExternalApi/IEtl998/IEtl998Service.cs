using game_x.share.ExternalApi.Etl998.Dtos.CancelTransfer;
using game_x.share.ExternalApi.Etl998.Dtos.ChangePassword;
using game_x.share.ExternalApi.Etl998.Dtos.ForwardGame;
using game_x.share.ExternalApi.Etl998.Dtos.IsAccountExist;
using game_x.share.ExternalApi.Etl998.Dtos.ModifyBettingLimit;
using game_x.share.ExternalApi.Etl998.Dtos.PrepareTransfer;
using game_x.share.ExternalApi.Etl998.Dtos.Register;
using game_x.share.ExternalApi.Etl998.Dtos.SearchRecord;
using game_x.share.ExternalApi.Etl998.Dtos.SearchTransfer;
using game_x.share.ExternalApi.Etl998.Dtos.Wallet;

namespace game_x.application.Contract.Infrastructure.ExternalApi.IEtl998;

public interface IEtl998Service
{
    Task<IReadOnlyCollection<Etl998RegisterResponse>> RegisterAsync(Etl998RegisterRequest req);
    Task<bool> IsAccountExistAsync(IsAccountExistRequest req);
    Task<IReadOnlyCollection<Etl998WalletResponse>> GetWalletAsync(Etl998WalletRequest req);
    Task<IReadOnlyCollection<Etl998TransferResponse>> PrepareTransferAsync(Etl998TransferRequest req);
    Task<IReadOnlyCollection<Etl998TransferResponse>> ConfirmTransferAsync(Etl998TransferRequest req);
    Task<IReadOnlyCollection<SearchTransferResponse>> SearchTransferAsync(SearchTransferRequest req);
    Task<IReadOnlyCollection<CancelTransferResponse>> CancelTransferAsync(CancelTransferRequest req);
    Task<IReadOnlyCollection<ForwardGameResponse>> ForwardGameAsync(ForwardGameRequest req);
    Task<IReadOnlyCollection<SearchRecordResponse>> SearchRecordAsync(SearchRecordRequest req);
    Task<bool> ChangePasswordAsync(ChangePasswordRequest req);
    Task<bool> ModifyBettingLimitAsync(ModifyBettingLimitRequest req);
}