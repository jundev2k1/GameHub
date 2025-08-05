using game_x.application.Contract.Infrastructure.Services.UserUsdtLedger;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.infrastructure.Services.UserUsdtLedger;

public sealed class UserUsdtLedgerService(IUserUsdtLedgerRepo userUsdtLedgerRepo ) : IUserUsdtLedgerService
{
    public async Task CreateForChainTransactionAsync(ChainTransaction chainTransaction)
    {
        var flowType = chainTransaction.Type == ChainTransactionType.Deposit ? UsdtFlowType.Deposit : UsdtFlowType.Withdrawal;

        decimal changeAmount = flowType switch
        {
            UsdtFlowType.Deposit => chainTransaction.Amount + chainTransaction.Fee,
            UsdtFlowType.Withdrawal => -(chainTransaction.Amount + chainTransaction.Fee),
            _ => throw new InvalidOperationException($"Unsupported flow type: {flowType}")
        };

        var previousLedger = await userUsdtLedgerRepo.GetLatestLedgerAsync(chainTransaction.UserId!);
        var previousBalance = previousLedger?.BalanceAfter ?? 0;

        var ledger = new domain.Entities.UserUsdtLedger
        {
            UserId = chainTransaction.UserId!,
            Timestamp = chainTransaction.ConfirmedAt,
            FlowType = flowType,
            SourceId = "",
            ChangeAmount = changeAmount,
            BalanceAfter = previousBalance + changeAmount,
            ChainTransactionId = chainTransaction.Id,
            MetaObject = new UserUsdtLedgerMeta
            {
                CounterpartyUserId = null,
                CounterpartyName = null
            }
        };

        await userUsdtLedgerRepo.AddAsync(ledger);
    }
}