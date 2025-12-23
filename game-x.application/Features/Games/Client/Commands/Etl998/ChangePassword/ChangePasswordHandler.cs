using game_x.application.Contract.Infrastructure.ExternalApi.IEtl998;
using game_x.share.ExternalApi.Etl998.Dtos.ChangePassword;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Games.Client.Commands.Etl998.ChangePassword;

public sealed class ChangePasswordHandler(
    IEtl998Service service,
    ILogger<ChangePasswordHandler> logger): ICommandHandler<ChangePasswordCommand, bool>
{
    public async Task<bool> Handle(ChangePasswordCommand cmd, CancellationToken ct = default)
    {
        try
        {
            var request = new ChangePasswordRequest
            {
                Account = cmd.AccountName, 
                Password = cmd.Password
            };
            return await service.ChangePasswordAsync(request);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw new BadRequestException(MessageCode.System.SystemError, ex.Message);
        }
    }
}