using game_x.share.ExternalApi.Etl998.Dtos.AccountBalance;
using game_x.share.ExternalApi.Etl998.Dtos.CancelTransfer;
using game_x.share.ExternalApi.Etl998.Dtos.ChangePassword;
using game_x.share.ExternalApi.Etl998.Dtos.ConfirmTransfer;
using game_x.share.ExternalApi.Etl998.Dtos.CreateAccount;
using game_x.share.ExternalApi.Etl998.Dtos.ForwardGame;
using game_x.share.ExternalApi.Etl998.Dtos.IsAccountExist;
using game_x.share.ExternalApi.Etl998.Dtos.ModifyBettingLimit;
using game_x.share.ExternalApi.Etl998.Dtos.PrepareTransfer;
using game_x.share.ExternalApi.Etl998.Dtos.SearchRecord;
using game_x.share.ExternalApi.Etl998.Dtos.SearchTransfer;

namespace game_x.application.Contract.Infrastructure.ExternalApi.IEtl998;

public interface IEtl998Service
{
    Task<IReadOnlyCollection<CreateAccountResponse>> CreateAccountAsync(CreateAccountRequest req);
    Task<bool> IsAccountExistAsync(IsAccountExistRequest req);
    Task<IReadOnlyCollection<AccountBalanceResponse>> GetAccountBalanceAsync(AccountBalanceRequest req);
    Task<IReadOnlyCollection<PrepareTransferResponse>> PrepareTransferAsync(PrepareTransferRequest req);
    Task<IReadOnlyCollection<ConfirmTransferResponse>> ConfirmTransferAsync(ConfirmTransferRequest req);
    Task<IReadOnlyCollection<SearchTransferResponse>> SearchTransferAsync(SearchTransferRequest req);
    Task<IReadOnlyCollection<CancelTransferResponse>> CancelTransferAsync(CancelTransferRequest req);
    Task<IReadOnlyCollection<ForwardGameResponse>> ForwardGameAsync(ForwardGameRequest req);
    Task<IReadOnlyCollection<SearchRecordResponse>> SearchRecordAsync(SearchRecordRequest req);
    Task<bool> ChangePasswordAsync(ChangePasswordRequest req);
    Task<bool> ModifyBettingLimitAsync(ModifyBettingLimitRequest req);
}