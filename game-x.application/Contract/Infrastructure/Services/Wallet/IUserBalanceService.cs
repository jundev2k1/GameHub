namespace game_x.application.Contract.Infrastructure.Services.Wallet;

public interface IUserBalanceService
{
    /// <summary>Freeze order</summary>
    void Freeze(UserBalance balance, decimal amount);
    /// <summary>Order canceled, unfrozen</summary>
    void Unfreeze(UserBalance balance, decimal amount);
    /// <summary>Complete the order and deduct the frozen amount</summary>
    void FinalizeFrozen(UserBalance balance, decimal amount);
    /// <summary>Increase the user's balance</summary>
    void IncreaseAmount(UserBalance balance, decimal amount);
    /// <summary>Decrease the user's balance</summary>
    void DecreaseAmount(UserBalance balance, decimal amount);
}