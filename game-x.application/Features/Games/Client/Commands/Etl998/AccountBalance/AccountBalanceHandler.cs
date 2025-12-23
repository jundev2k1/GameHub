using game_x.application.Contract.Infrastructure.ExternalApi.IEtl998;
using game_x.share.ExternalApi.Etl998.Dtos.AccountBalance;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Games.Client.Commands.Etl998.AccountBalance;

public sealed class AccountBalanceHandler(
    IEtl998Service service,
    ILogger<AccountBalanceHandler> logger): ICommandHandler<AccountBalanceCommand, IReadOnlyCollection<AccountBalanceResponse>>
{
    public async Task<IReadOnlyCollection<AccountBalanceResponse>> Handle(AccountBalanceCommand cmd, CancellationToken ct = default)
    {
        try
        {
            var request = new AccountBalanceRequest
            {
                Account = cmd.AccountName, 
                Password = cmd.Password
            };
            return await service.GetAccountBalanceAsync(request);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw new BadRequestException(MessageCode.System.SystemError, ex.Message);
        }
    }
}