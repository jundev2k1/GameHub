using game_x.application.Features.Accounts.User.Dtos;

namespace game_x.application.Contract.Infrastructure.Caching;

public interface IWalletManagerCacheService
{
    Task<UserWalletDto> GetWalletAsync(string userId);

    Task<UserWalletExternalItemDto> GetExternalWalletAsync(string userId, Guid platformId);

    Task RefreshWalletAsync(string userId);

    Task RefreshInternalWalletAsync(string userId, Guid walletId, decimal amount, decimal frozenAmount);

    Task RefreshExternalWalletAsync(string userId, Guid platformId, decimal balance);
}
