using game_x.application.Contract.Infrastructure.ExternalApi.IEtl998;
using game_x.share.ExternalApi.Etl998.Dtos.SearchRecord;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Games.Client.Commands.Etl998.SearchRecord;

public sealed class SearchRecordHandler(
    IEtl998Service service,
    ILogger<SearchRecordHandler> logger): ICommandHandler<SearchRecordCommand, IReadOnlyCollection<SearchRecordResponse>>
{
    public async Task<IReadOnlyCollection<SearchRecordResponse>> Handle(SearchRecordCommand cmd, CancellationToken ct = default)
    {
        try
        {
            var request = new SearchRecordRequest
            {
                Account = cmd.AccountName, 
                Password = cmd.Password,
                DateStart = cmd.DateStart,
                DateEnd = cmd.DateEnd,
                PageIndex = cmd.PageIndex,
                PageSize = cmd.PageSize
            };
            return await service.SearchRecordAsync(request);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw new BadRequestException(MessageCode.System.SystemError, ex.Message);
        }
    }
}