using game_x.share.ExternalApi.GameProvider.Dtos;

namespace game_x.share.ExternalApi.GameProvider.Dtos.Deposit;

public record WalletDepositResponse(
    bool issuccess,
    string? message = null,
    string? errorcode = null,
    string? errormessage = null
 ) : GameProviderBaseResponse(issuccess, message, errorcode, errormessage);