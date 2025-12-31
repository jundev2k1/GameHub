namespace game_x.application.Contract.Infrastructure.ExternalApi.SasSlot;

public interface ISasSlotService
{
    Task<string> LoginAsync(string account, string nickname);

    Task RegisterAsync(string account, string nickname);

    Task<decimal> GetWalletAsync(string account);

    Task DepositAsync(string account, decimal amount, string sno);

    Task WithdrawalAsync(string account, decimal amount, string sno);

    Task DeletePublicKeyAsync();
}
