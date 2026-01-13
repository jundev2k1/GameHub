using game_x.application.Features.Transactions.Dtos;
using game_x.application.Features.Transactions.Enums;
using game_x.share.Extensions;
using System.Linq.Expressions;

namespace game_x.application.Extensions.FilterExtensions;

public static class TransactionFilterExtensions
{
    public static readonly Dictionary<string, Func<object, Expression<Func<Transaction, bool>>>> InternalOptions =
        new()
        {
            ["statuses"] = CreateStatusFilter
        };

    public static readonly Dictionary<string, Func<object, Expression<Func<Transaction, bool>>>> ExternalOptions =
        new()
        {
            ["statuses"] = CreateStatusFilter,
            ["platforms"] = CreatePlatformFilter
        };

    public static readonly Dictionary<string, Func<object, Expression<Func<WalletTransactionDto, bool>>>> WalletTransactionOptions =
        new()
        {
            ["tabType"] = CreateTabTypeFilter,
        };

    /// <summary>Builds a filter by multiple statuses.</summary>
    private static Expression<Func<Transaction, bool>> CreateStatusFilter(object value)
    {
        var raw = value.ToStringOrEmpty();
        if (raw.IsNullOrEmpty())
            return _ => true;

        var parsedStatuses = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => Enum.TryParse<TransactionStatus>(s, true, out _))
            .Select(s => Enum.Parse<TransactionStatus>(s, true))
            .Distinct()
            .ToList();
        
        if (parsedStatuses.Count == 0)
            return _ => false;
        
        bool includePending = parsedStatuses.Remove(TransactionStatus.Pending);
        bool includeExpired = parsedStatuses.Remove(TransactionStatus.Expired);
        
        return transaction =>
            parsedStatuses.Contains(transaction.Status)
            || (includePending
                && transaction.Status == TransactionStatus.Pending
                && transaction.ExpiredAt == null)
            || (includeExpired
                && transaction.Status == TransactionStatus.Pending
                && transaction.ExpiredAt != null);
    }

    /// <summary>Builds a filter by platform</summary>
    private static Expression<Func<Transaction, bool>> CreatePlatformFilter(object value)
    {
        var raw = value.ToStringOrEmpty();
        if (raw.IsNullOrEmpty())
            return _ => true;

        var statusList = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(s => Guid.TryParse(s, out _))
            .Select(Guid.Parse)
            .ToArray();
        if (statusList.Length == 0)
            return _ => false;

        return transaction => (transaction.TransactionExternal != null)
            && statusList.Contains(transaction.TransactionExternal.GamePlatform.PublicId);
    }

    private static Expression<Func<WalletTransactionDto, bool>> CreateTabTypeFilter(object value)
    {
        var raw = value.ToStringOrEmpty();
        if (raw.IsNullOrEmpty() || !Enum.TryParse<TransactionTabType>(raw, true, out var type))
            return _ => true;

        if (type == TransactionTabType.Cash)
        {
            return tx => !new[]
            {
                TransactionType.BalanceAdjustment, 
                TransactionType.Init
            }.Contains(tx.Type);
        }

        if (type == TransactionTabType.Credit)
        {
            return tx =>
                new[]
                {
                    TransactionSourceType.G598SnoGameProvider,
                    TransactionSourceType.BaccaratGameProvider,
                    TransactionSourceType.Elt998GameProvider,
                    TransactionSourceType.SasSlotProvider,
                    TransactionSourceType.AtgProvider,
                }.Contains(tx.SourceType);
        }

        return _ => true;
    }
}