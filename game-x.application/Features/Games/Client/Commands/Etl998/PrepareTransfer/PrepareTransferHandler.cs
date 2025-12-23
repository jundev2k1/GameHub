using game_x.application.Contract.Infrastructure.ExternalApi.IEtl998;
using game_x.share.ExternalApi.Etl998.Dtos.PrepareTransfer;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Games.Client.Commands.Etl998.PrepareTransfer;

public sealed class PrepareTransferHandler(
    IEtl998Service service,
    ILogger<PrepareTransferHandler> logger): ICommandHandler<PrepareTransferCommand, IReadOnlyCollection<PrepareTransferResponse>>
{
    public async Task<IReadOnlyCollection<PrepareTransferResponse>> Handle(PrepareTransferCommand cmd, CancellationToken ct = default)
    {
        try
        {
            var request = new PrepareTransferRequest
            {
                Account = cmd.AccountName, 
                Password = cmd.Password,
                CustomerOrderId = cmd.CustomerOrderId,
                Credit = cmd.Credit,
                Type = cmd.CreditType
            };
            return await service.PrepareTransferAsync(request);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw new BadRequestException(MessageCode.System.SystemError, ex.Message);
        }
    }
}