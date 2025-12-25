using game_x.application.Contract.Infrastructure.ExternalApi.IEtl998;
using game_x.share.ExternalApi.Etl998.Dtos.ForwardGame;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Games.Client.Commands.Etl998.ForwardGame;

public sealed class ForwardGameHandler(
    IEtl998Service service,
    ILogger<ForwardGameHandler> logger): ICommandHandler<ForwardGameCommand, IReadOnlyCollection<ForwardGameResponse>>
{
    public async Task<IReadOnlyCollection<ForwardGameResponse>> Handle(ForwardGameCommand cmd, CancellationToken ct = default)
    {
        try
        {
            var request = new ForwardGameRequest
            {
                Account = cmd.AccountName, 
                Password = cmd.Password,
                Dm = cmd.Dm
            };
            return await service.ForwardGameAsync(request);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw new BadRequestException(MessageCode.System.SystemError, ex.Message);
        }
    }
}