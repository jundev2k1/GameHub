namespace game_x.application.Contract.Infrastructure.Services.Wallet;

public interface IUserBalanceService
{
    // Freeze order
    void Freeze(UserBalance balance, decimal amount);
    
    // Order canceled, unfrozen
    void Unfreeze(UserBalance balance, decimal amount);

    // Complete the order and deduct the frozen amount
    void FinalizeFrozen(UserBalance balance, decimal amount);
    
    // Add amount to balance (for deposits)
    void AddAmount(UserBalance balance, decimal amount);
}