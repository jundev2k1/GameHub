using game_x.application.Contract.Infrastructure.ExternalApi.IEtl998;
using game_x.share.ExternalApi.Etl998.Dtos.IsAccountExist;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Games.Client.Commands.Etl998.CheckAccountExistence;

public sealed class CheckAccountExistenceHandler(
    IEtl998Service service,
    ILogger<CheckAccountExistenceHandler> logger): ICommandHandler<CheckAccountExistenceCommand, bool>
{
    public async Task<bool> Handle(CheckAccountExistenceCommand cmd, CancellationToken ct = default)
    {
        try
        {
            var request = new IsAccountExistRequest { Account = cmd.AccountName };
            return await service.IsAccountExistAsync(request);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw new BadRequestException(MessageCode.System.SystemError, ex.Message);
        }
    }
}