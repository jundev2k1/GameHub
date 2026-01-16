using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Extensions.FilterExtensions;
using game_x.application.Features.Transactions.Dtos;
using game_x.application.Features.Transactions.Enums;

namespace game_x.application.Features.Transactions.Client.Queries.GetMyWalletTransactions;

public sealed class GetMyWalletTransactionsHandler(
    ICriteriaBuilder<WalletTransactionDto> builder,
    ITransactionRepo transactionRepo,
    IUserAccessor userAccessor) : IQueryHandler<GetMyWalletTransactionsQuery, PaginationResult<WalletTransactionDto>>
{
    public async Task<PaginationResult<WalletTransactionDto>> Handle(GetMyWalletTransactionsQuery request, CancellationToken ct = default)
    {
        Enum.TryParse<TransactionTabType>(request.Mode, true, out var mode);
        var isCreditMode = mode == TransactionTabType.Credit;

        var userId = userAccessor.GetUserId();
        var result = await transactionRepo.GetMyWalletTransactionsAsync(
            userId,
            query => builder.Apply(
                query.Where(q => q.Status == TransactionStatus.Completed),
                request.Filters,
                request.Sorts,
                options: TransactionFilterExtensions.WalletTransactionOptions),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);

        MapAddressWalletForItems(result, isCreditMode);
        return result;
    }

    private static void MapAddressWalletForItems(PaginationResult<WalletTransactionDto> result, bool isCreditMode)
    {
        var cashKey = "cash";

        foreach (var item in result.Items)
        {
            var isGameTransaction = item.SourceType == WalletSourceType.External;
            var isUxmTransaction = item.SourceType == WalletSourceType.Internal;

            var isDeposit = item.Type == TransactionType.Deposit;
            var isWithdrawal = item.Type == TransactionType.Withdrawal;
            var isDonationOut = item.Type == TransactionType.TransferSent;
            var isBalanceAdjustment = item.Type == TransactionType.BalanceAdjustment;

            // Wallet ledgers tab will display the actual amount instead of transaction amount
            item.Amount = item.ActualAmount;

            #region Transaction address mapping
            var isFromCash = (isWithdrawal && isUxmTransaction)
                || (isDeposit && isGameTransaction);
            if (isFromCash)
                item.From = cashKey;

            var isFromPlatform = isWithdrawal && isGameTransaction;
            if (isFromPlatform)
                item.From = item.GamePlatformName;

            var isToCash = (isWithdrawal && isGameTransaction)
                || (isDeposit && isUxmTransaction);
            if (isToCash)
                item.To = cashKey;

            var isToPlatform = isDeposit && isGameTransaction;
            if (isToPlatform)
                item.To = item.GamePlatformName; 
            #endregion

            if (isWithdrawal && isUxmTransaction)
                item.Amount = Math.Abs(item.Amount) * -1;

            // Cash tab: the amount of donation transaction must be a negative value
            if (!isCreditMode && isDonationOut)
                item.Amount = Math.Abs(item.Amount) * -1;

            #region Only handle in game transactions
            if (!isGameTransaction || (!isWithdrawal && !isDeposit && !isBalanceAdjustment))
                continue;

            // Map transaction amount according Credit mode
            if (isCreditMode)
            {
                if (isWithdrawal)
                    item.Amount = Math.Abs(item.Amount) * -1;

                item.BalanceAfter = item.GameBalanceAfter;
            }

            // Map transaction amount according Cash mode
            if (!isCreditMode && isDeposit)
                item.Amount = Math.Abs(item.Amount) * -1; 
            #endregion
        }
    }
}