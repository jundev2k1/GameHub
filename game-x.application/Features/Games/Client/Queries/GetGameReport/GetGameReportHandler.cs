using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.share.Extensions;
using game_x.share.ExternalApi.GameProvider.Dtos.Report;

namespace game_x.application.Features.Games.Client.Queries.GetGameReport;

public sealed class GetGameReportHandler(
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IGameProviderService gameProvider) : IQueryHandler<GetGameReportQuery, GameReportResponse>
{
    public async Task<GameReportResponse> Handle(GetGameReportQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var userExtend = await userRepo.GetUserExtendAsync(userId, ct);
        var reportRequest = new GameReportRequest
        {
            StartDate = request.StartDate?.ToString("yyyy-MM-dd hh:MM:ss") ?? string.Empty,
            EndDate = request.EndDate?.ToString("yyyy-MM-dd hh:MM:ss") ?? string.Empty
        };
        var report = await gameProvider.GetReportAsync(reportRequest, userExtend.GameProviderAccount);
        return report;
    }
}
