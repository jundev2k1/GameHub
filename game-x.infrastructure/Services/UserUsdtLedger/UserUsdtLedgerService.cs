using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.Services.UserUsdtLedger;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.infrastructure.Services.UserUsdtLedger;

public sealed class UserUsdtLedgerService
    (IUserUsdtLedgerRepo userUsdtLedgerRepo) : IUserUsdtLedgerService, IServices
{
    public async Task CreateForChainTransactionAsync(ChainTransaction transaction)
    {
        var flowType = transaction.Type == ChainTransactionType.Deposit ? UsdtFlowType.Deposit : UsdtFlowType.Withdrawal;

        decimal changeAmount = flowType switch
        {
            UsdtFlowType.Deposit => transaction.Amount + transaction.Fee,
            UsdtFlowType.Withdrawal => -(transaction.Amount + transaction.Fee),
            _ => throw new InvalidOperationException($"Unsupported flow type: {flowType}")
        };

        var previousLedger = await userUsdtLedgerRepo.GetLatestLedgerAsync(transaction.UserId!);
        var previousBalance = previousLedger?.BalanceAfter ?? 0;

        var ledger = new domain.Entities.UserUsdtLedger
        {
            UserId = transaction.UserId!,
            Timestamp = transaction.ConfirmedAt ?? DateTime.UtcNow,
            FlowType = flowType,
            SourceId = "",
            ChangeAmount = changeAmount,
            BalanceAfter = previousBalance + changeAmount,
            ChainTransactionId = transaction.Id,
            StatusAtEvent = transaction.Status.ToString().ToLower(),
            MetaObject = new UserUsdtLedgerMeta
            {
                CounterpartyUserId = null,
                CounterpartyName = null
            }
        };

        await userUsdtLedgerRepo.AddAsync(ledger);
    }

    public async Task CreateForGameTransactionAsync(GameTransaction transaction)
    {
        var flowType = transaction.Type == GameTransactionType.Deposit ? UsdtFlowType.GameDeposit : UsdtFlowType.GameWithdrawal;

        decimal changeAmount = flowType switch
        {
            UsdtFlowType.GameDeposit => -transaction.Amount,
            UsdtFlowType.GameWithdrawal => transaction.Amount,
            _ => throw new InvalidOperationException($"Unsupported flow type: {flowType}")
        };

        var previousLedger = await userUsdtLedgerRepo.GetLatestLedgerAsync(transaction.UserId);
        var previousBalance = previousLedger?.BalanceAfter ?? 0;

        var ledger = new domain.Entities.UserUsdtLedger
        {
            UserId = transaction.UserId,
            Timestamp = transaction.CreatedAt,
            FlowType = flowType,
            SourceId = "",
            ChangeAmount = changeAmount,
            BalanceAfter = previousBalance + changeAmount,
            GameTransactionId = transaction.Id,
            Type = LedgerType.G598SnoGameProvider,
            MetaObject = new UserUsdtLedgerMeta
            {
                CounterpartyUserId = null,
                CounterpartyName = null
            }
        };

        await userUsdtLedgerRepo.AddAsync(ledger);
    }

}