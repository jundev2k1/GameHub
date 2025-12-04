using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.GameBaccarat;
using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnGameRegister;
using game_x.application.Events.OnUserBalanceUpdated;
using game_x.share.Extensions;
using game_x.share.ExternalApi.GameBaccarat.Dtos.Login;
using game_x.share.ExternalApi.GameProvider.Dtos.Login;
using game_x.share.ExternalApi.GameProvider.Dtos.Logout;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.application.Features.Games.Client.Commands.LoginGame;

public sealed class LoginGameHandler(
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IGameProviderService gameProvider,
    IGameBaccaratService gameBaccarat,
    IGameAesEncryptor aesEncryptor,
    IGameProviderCacheService gameProviderCache,
    IOptions<GameProviderSettings> gameSettings,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<LoginGameCommand, LoginGameResult>
{
    public async Task<LoginGameResult> Handle(LoginGameCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var targetUser = await userRepo.GetUserByIdAsync(userId, ct);

        // Check: email must be confirmed before requesting password reset
        if (!targetUser.EmailConfirmed)
            throw new BadRequestException(MessageCode.User.UserNotConfirmed);

        // Create new external account if none exists
        if (!CheckExistAccount(targetUser.UserExtend, request.GamePlatformId!.Value))
        {
            var @event = new OnGameRegisterEvent(request.GamePlatformId.Value, userId);
            await eventDispatcher.Publish(@event, ct);

            // Retry after account created
            targetUser = await userRepo.GetUserByIdAsync(userId, ct);
        }
        else
        {
            // Loggout if user already login
            await LogoutGameAsync(request.GamePlatformId.Value, targetUser.UserExtend!);
        }

        // Login from external API
        var url = await LoginGameAsync(request.GamePlatformId.Value, targetUser.UserExtend!, request, ct)
            ?? throw new BadRequestException($"GamePlatformId({request.GamePlatformId.Value}) is not supported.");

        var gameEmbededLink = ConvertEmbededLink(url);
        return new LoginGameResult(gameEmbededLink);
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

    private async Task<string?> LoginGameAsync(
        Guid gamePlatformId,
        UserExtend usrex,
        LoginGameCommand request,
        CancellationToken ct)
    {
        if (gamePlatformId == GameConstants.PLATFORM_ID_G598)
        {
            var externalRequest = new GameLoginRequest
            {
                Account = usrex.GameProviderAccount,
                Passwd = aesEncryptor.Decrypt(usrex.GameProviderPassword),
                Gamecode = request.GameCode,
                Address = request.Address,
                Locale = request.Locale,
                ReturnUrl = request.ReturnUrl,
            };
            var result = await gameProvider.LoginAsync(externalRequest, request.IpAddress!);

            // Set Login state
            gameProviderCache.SetIsLoggedIn(externalRequest.Account, true);
            // Reset language
            gameProviderCache.SetLanguage(externalRequest.Account, request.Locale);

            // Update user balance
            await eventDispatcher.Publish(new OnUserBalanceUpdatedEvent(usrex.Id, gamePlatformId), ct);
            return result.Url;
        }

        if (gamePlatformId == GameConstants.PLATFORM_ID_GAMEBACCARAT)
        {
            var externalRequest = new GameBaccaratLoginRequest
            {
                Account = usrex.GameBaccaratAccount,
                Password = aesEncryptor.Decrypt(usrex.GameBaccaratPassword),
                Gamecode = request.GameCode
            };
            var result = await gameBaccarat.LoginAsync(externalRequest);

            return result.Url;
        }

        return null;
    }

    private async Task LogoutGameAsync(Guid gamePlatformId, UserExtend usrex)
    {
        if (gamePlatformId == GameConstants.PLATFORM_ID_G598)
        {
            if (!gameProviderCache.GetIsLoggedIn(usrex.GameProviderAccount))
                return;

            var logoutRequest = new GameLogoutRequest
            {
                Account = usrex.GameProviderAccount,
            };
            await gameProvider.LogoutAsync(logoutRequest);
            gameProviderCache.SetIsLoggedIn(usrex.GameProviderAccount, false);
        }
    }

    private string ConvertEmbededLink(string url)
    {
        var domain = gameSettings.Value.RevertProxyUrl;
        if (domain.IsNullOrWhiteSpace()) return url;

        var uri = new Uri(url);
        return $"{domain}{uri.PathAndQuery}";
    }
}
