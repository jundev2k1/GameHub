using game_x.application.Contract.Infrastructure.ExternalApi.IEtl998;
using game_x.share.ExternalApi.Etl998.Dtos.SearchTransfer;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Games.Client.Commands.Etl998.SearchTransfer;

public sealed class SearchTransferHandler(
    IEtl998Service service,
    ILogger<SearchTransferHandler> logger): ICommandHandler<SearchTransferCommand, IReadOnlyCollection<SearchTransferResponse>>
{
    public async Task<IReadOnlyCollection<SearchTransferResponse>> Handle(SearchTransferCommand cmd, CancellationToken ct = default)
    {
        try
        {
            var request = new SearchTransferRequest
            {
                Account = cmd.AccountName, 
                Password = cmd.Password,
                CustomerOrderId = cmd.CustomerOrderId,
                DateStart = cmd.DateStart,
                DateEnd = cmd.DateEnd,
            };
            return await service.SearchTransferAsync(request);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw new BadRequestException(MessageCode.System.SystemError, ex.Message);
        }
    }
}