using game_x.share.ExternalApi.Uxm.Dtos;
using Refit;

namespace game_x.infrastructure.ExternalApi.Uxm;

public interface IUxmApi
{
    [Post("/v2/order/tron/usdt/deposit")]
    Task<ApiResponse<SecureResponse<CreateChainTransactionDepositResponseData>>> CreateProxyChainTransactionDepositAsync(
    [Body] SecureRequest<CreateChainTransactionDepositRequestData> request);

}
