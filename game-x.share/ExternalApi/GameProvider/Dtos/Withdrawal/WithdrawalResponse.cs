using game_x.share.ExternalApi.GameProvider.Dtos;

namespace game_x.share.ExternalApi.GameProvider.Dtos.Withdrawal;

public record WalletWithdrawalResponse(
    bool issuccess,
    string? message = null,
    string? errorcode = null,
    string? errormessage = null
 ) : GameProviderBaseResponse(issuccess, message, errorcode, errormessage);