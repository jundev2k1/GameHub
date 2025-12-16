using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnGameRegister;
using game_x.share.Extensions;

namespace game_x.application.Features.Games.Services;

public interface IGamePlatformService
{
    Task<User> EnsureExternalAccountCreatedAsync(
        User user,
        Guid gamePlatformId,
        Func<Task>? fallbackAction = null,
        CancellationToken ct = default);
}

public sealed class GamePlatformService(
    IUserRepo userRepo,
    IApplicationEventDispatcher eventDispatcher) : IGamePlatformService, IServices
{
    public async Task<User> EnsureExternalAccountCreatedAsync(
        User user,
        Guid gamePlatformId,
        Func<Task>? fallbackAction = null,
        CancellationToken ct = default)
    {
        if (CheckExistAccount(user.UserExtend, gamePlatformId))
        {
            if (fallbackAction is not null)
                await fallbackAction();

            return user;
        }

        var gameRegisterEvent = new OnGameRegisterEvent(gamePlatformId, user.Id);
        await eventDispatcher.Publish(gameRegisterEvent, ct);

        // Retry after account created
        return await userRepo.GetUserByIdAsync(user.Id, ct);
    }

    private static bool CheckExistAccount(UserExtend? usrex, Guid gamePlatformId)
    {
        if (usrex is null) return false;

        if ((gamePlatformId == GameConstants.PLATFORM_ID_G598)
            && (usrex.GameProviderAccount.IsNullOrWhiteSpace() || usrex.GameProviderPassword.IsNullOrWhiteSpace()))
            return false;

        if ((gamePlatformId == GameConstants.PLATFORM_ID_GAMEBACCARAT)
            && (usrex.GameBaccaratAccount.IsNullOrWhiteSpace() || usrex.GameBaccaratPassword.IsNullOrWhiteSpace()))
            return false;

        return true;
    }
}
