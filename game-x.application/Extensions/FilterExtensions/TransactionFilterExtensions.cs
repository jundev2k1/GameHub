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
            ["type"] = CreateTabTypeFilter,
        };

    /// <summary>Builds a filter by multiple statuses.</summary>
    private static Expression<Func<Transaction, bool>> CreateStatusFilter(object value)
    {
        var raw = value.ToStringOrEmpty();
        if (raw.IsNullOrEmpty())
            return _ => true;

        var statusList = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim().ToUpperInvariant())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();

        if (statusList.Count == 0)
            return _ => false;

        var validStatuses = statusList.Select(item =>
        {
            Enum.TryParse<TransactionStatus>(item, ignoreCase: true, out var status);
            return status;
        }).ToList();

        return transaction => validStatuses.Contains(transaction.Status);
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
        if (raw.IsNullOrEmpty() || !Enum.TryParse<TransactionTabType>(raw, out var type))
            return _ => true;

        if (type == TransactionTabType.Cash)
        {
            return tx => tx.Type != TransactionType.BalanceAdjustment
                && tx.Type != TransactionType.Init;
        }
        if (type == TransactionTabType.Credit)
        {
            return tx => tx.SourceType == TransactionSourceType.G598SnoGameProvider
                && tx.SourceType == TransactionSourceType.BaccaratGameProvider;
        }

        return _ => true;
    }
}
