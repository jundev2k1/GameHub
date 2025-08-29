namespace game_x.application.Contract.Infrastructure.Services.Statistics.Admin;

public interface IAdminStatistics
{
    Task<(int WithdrawalCount, int KycCount, int BankAccountCount)> GetUnderReviewStatisticsAsync(CancellationToken ct = default);
}
