namespace game_x.application.Contract.Infrastructure.Services.UserUsdtLedger;

public interface IUserUsdtLedgerService
{
    Task CreateForChainTransactionAsync(ChainTransaction transaction);
    Task CreateForGameTransactionAsync(GameTransaction transaction);

}