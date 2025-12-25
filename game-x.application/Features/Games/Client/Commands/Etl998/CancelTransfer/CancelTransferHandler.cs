using game_x.application.Contract.Infrastructure.ExternalApi.IEtl998;
using game_x.share.ExternalApi.Etl998.Dtos.CancelTransfer;
using game_x.share.ExternalApi.Etl998.Dtos.SearchTransfer;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Games.Client.Commands.Etl998.CancelTransfer;

public sealed class CancelTransferHandler(
    IEtl998Service service,
    ILogger<CancelTransferHandler> logger): ICommandHandler<CancelTransferCommand, IReadOnlyCollection<CancelTransferResponse>>
{
    public async Task<IReadOnlyCollection<CancelTransferResponse>> Handle(CancelTransferCommand cmd, CancellationToken ct = default)
    {
        try
        {
            var request = new CancelTransferRequest
            {
                Account = cmd.AccountName, 
                Password = cmd.Password,
                CustomerOrderId = cmd.CustomerOrderId,
                DateStart = cmd.DateStart,
                DateEnd = cmd.DateEnd,
            };
            return await service.CancelTransferAsync(request);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw new BadRequestException(MessageCode.System.SystemError, ex.Message);
        }
    }
}