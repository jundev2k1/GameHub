using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.Services.Statistics.Admin;
using Microsoft.EntityFrameworkCore;

namespace game_x.infrastructure.Services.Statistics;

public sealed class AdminStatistics(GameXContext context) : IAdminStatistics, IServices
{
    public async Task<(int WithdrawalCount, int KycCount, int BankAccountCount)> GetUnderReviewStatisticsAsync(CancellationToken ct = default)
    {
        var pendingWithdrawals = await context.Transactions
            .AsNoTracking()
            .CountAsync(tx => tx.Type == TransactionType.Withdrawal && tx.Status == TransactionStatus.Pending, ct);

        var pendingKycs = await context.UserKycs
            .AsNoTracking()
            .CountAsync(kyc => kyc.User.Status == UserStatus.Active && kyc.Status == KycStatus.UnderReview, ct);

        var pendingBankAccounts = await context.UserBankAccounts
            .AsNoTracking()
            .CountAsync(ba => ba.User.Status == UserStatus.Active && ba.Status == UserBankAccountStatus.UnderReview, ct);

        return (pendingWithdrawals, pendingKycs, pendingBankAccounts);
    }
}
