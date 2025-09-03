using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.Services.Wallet;
using game_x.application.Exceptions;

namespace game_x.infrastructure.Services.Wallet;

public sealed class UserBalanceService : IUserBalanceService, IServices
{
    private readonly decimal _withdrawalFree = 0m;
    public decimal GetWithdrawalFree() => _withdrawalFree;
    
    public void Freeze(UserBalance balance, decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount, nameof(amount));
  
        if (balance.Amount < amount)
            throw new BadRequestException(MessageCode.Accounting.InsufficientBalance, "Insufficient available balance to freeze.");

        balance.Amount -= amount;
        balance.FrozenAmount += amount;
    }

    public void Unfreeze(UserBalance balance, decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount, nameof(amount));

        if (balance.FrozenAmount < amount)
            throw new BadRequestException(MessageCode.Accounting.InsufficientFrozenBalance, "Insufficient frozen balance to unfreeze.");

        balance.FrozenAmount -= amount;
        balance.Amount += amount;
    }

    public void FinalizeFrozen(UserBalance balance, decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount, nameof(amount));

        if (balance.FrozenAmount < amount)
            throw new BadRequestException(MessageCode.Accounting.InsufficientFrozenBalance, "Insufficient frozen balance to finalize.");

        balance.FrozenAmount -= amount;
    }

    public void IncreaseAmount(UserBalance balance, decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount, nameof(amount));
        balance.Amount += amount;
    }
    
    public void DecreaseAmount(UserBalance balance, decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount, nameof(amount));
        balance.Amount -= amount;
    }
}