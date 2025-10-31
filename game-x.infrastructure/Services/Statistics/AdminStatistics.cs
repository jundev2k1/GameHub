using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.Services.Statistics.Admin;
using Microsoft.EntityFrameworkCore;

namespace game_x.infrastructure.Services.Statistics;

public sealed class AdminStatistics(GameXContext context) : IAdminStatistics, IServices
{
    public async Task<(int WithdrawalCount, int KycCount, int BankAccountCount)> GetUnderReviewStatisticsAsync(CancellationToken ct = default)
    {
        var statistic = await context.UserBankAccounts
            .OrderByDescending(ba => ba.CreatedAt)
            .GroupBy(_ => 1)
            .Select(baGroup => Tuple.Create(
                context.Transactions
                    .Count(tx => (tx.Type == TransactionType.Withdrawal) && (tx.Status == TransactionStatus.Pending)),
                context.UserKycs
                    .Count(kyc => (kyc.User.Status == UserStatus.Active) && (kyc.Status == KycStatus.UnderReview)),
                baGroup.Count(ba => (ba.User.Status == UserStatus.Active) && (ba.Status == UserBankAccountStatus.UnderReview))
            ))
            .FirstOrDefaultAsync(ct);
        return (statistic?.Item1 ?? 0, statistic?.Item2 ?? 0, statistic?.Item3 ?? 0);
    }
}
