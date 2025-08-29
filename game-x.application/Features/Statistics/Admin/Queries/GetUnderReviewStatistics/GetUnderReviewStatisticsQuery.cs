namespace game_x.application.Features.GetUnderReviewStatistics.Admin.Queries.GetTransactionStatistics;

public record GetUnderReviewStatisticsQuery : IQuery<GetUnderReviewStatisticsResult>;

public record GetUnderReviewStatisticsResult(
    int WithdrawalCount,
    int KycCount,
    int BankAccountCount);
