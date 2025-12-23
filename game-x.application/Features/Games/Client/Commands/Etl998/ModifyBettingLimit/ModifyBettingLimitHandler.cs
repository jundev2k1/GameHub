using game_x.application.Contract.Infrastructure.ExternalApi.IEtl998;
using game_x.share.ExternalApi.Etl998.Dtos.ModifyBettingLimit;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Games.Client.Commands.Etl998.ModifyBettingLimit;

public sealed class ModifyBettingLimitHandler(
    IEtl998Service service,
    ILogger<ModifyBettingLimitHandler> logger): ICommandHandler<ModifyBettingLimitCommand, bool>
{
    public async Task<bool> Handle(ModifyBettingLimitCommand cmd, CancellationToken ct = default)
    {
        try
        {
            var request = new ModifyBettingLimitRequest
            {
                Account = cmd.AccountName, 
                Tables = cmd.Tables
            };
            return await service.ModifyBettingLimitAsync(request);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw new BadRequestException(MessageCode.System.SystemError, ex.Message);
        }
    }
}