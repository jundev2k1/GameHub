using game_x.application.Contract.Infrastructure.Services.Statistics.Admin;
using game_x.application.Features.GetUnderReviewStatistics.Admin.Queries.GetTransactionStatistics;

namespace game_x.application.Features.Statistics.Admin.Queries.GetUnderReviewStatistics;

public sealed class GetUnderReviewStatisticsHandler(
    IAdminStatistics adminStatistics) : IQueryHandler<GetUnderReviewStatisticsQuery, GetUnderReviewStatisticsResult>
{
    public async Task<GetUnderReviewStatisticsResult> Handle(GetUnderReviewStatisticsQuery request, CancellationToken ct = default)
    {
        var (withdrawalCount, kycCount, bankAccountCount) = await adminStatistics.GetUnderReviewStatisticsAsync(ct);
        return new GetUnderReviewStatisticsResult(withdrawalCount, kycCount, bankAccountCount);
    }
}
