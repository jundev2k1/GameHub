using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.Services.Wallet;

namespace game_x.infrastructure.Services.Wallet;

public sealed class UserBalanceService : IUserBalanceService, IServices
{
    public void Freeze(UserBalance balance, decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than 0.");

        if (balance.Amount < amount)
            throw new InvalidOperationException("Insufficient available balance to freeze.");

        balance.Amount -= amount;
        balance.FrozenAmount += amount;
    }

    public void Unfreeze(UserBalance balance, decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        if (balance.FrozenAmount < amount)
            throw new InvalidOperationException("Insufficient frozen balance to unfreeze.");

        balance.FrozenAmount -= amount;
        balance.Amount += amount;
    }

    public void FinalizeFrozen(UserBalance balance, decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        if (balance.FrozenAmount < amount)
            throw new InvalidOperationException("Insufficient frozen balance to finalize.");

        balance.FrozenAmount -= amount;
    }
}