using game_x.application.Contract.Infrastructure.ExternalApi.IEtl998;
using game_x.share.ExternalApi.Etl998.Dtos.ConfirmTransfer;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Games.Client.Commands.Etl998.ConfirmTransfer;

public sealed class ConfirmTransferHandler(
    IEtl998Service service,
    ILogger<ConfirmTransferHandler> logger): ICommandHandler<ConfirmTransferCommand, IReadOnlyCollection<ConfirmTransferResponse>>
{
    public async Task<IReadOnlyCollection<ConfirmTransferResponse>> Handle(ConfirmTransferCommand cmd, CancellationToken ct = default)
    {
        try
        {
            var request = new ConfirmTransferRequest
            {
                Account = cmd.AccountName, 
                Password = cmd.Password,
                CustomerOrderId = cmd.CustomerOrderId,
                Credit = cmd.Credit,
                Type = cmd.CreditType,
            };
            return await service.ConfirmTransferAsync(request);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw new BadRequestException(MessageCode.System.SystemError, ex.Message);
        }
    }
}