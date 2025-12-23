using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.GameBaccarat;
using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnUserBalanceUpdated;
using game_x.application.Features.Games.Services;
using game_x.application.Features.UserGameSessions.Dtos;
using game_x.share.Extensions;
using game_x.share.ExternalApi.GameBaccarat.Dtos.Login;
using game_x.share.ExternalApi.GameProvider.Dtos.Login;
using game_x.share.Helper;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.application.Features.Games.Client.Commands.LoginGame;

public sealed class LoginGameHandler(
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IGameProviderService gameProvider,
    IGameBaccaratService gameBaccarat,
    IGameAesEncryptor gameAesEncryptor,
    IAesEncryptor aesEncryptor,
    IGameProviderCacheService gameProviderCache,
    IGamePlatformService gamePlatformService,
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

        var gameInfo = gameProviderCache.GameList.FirstOrDefault(gl => gl.GameCode == request.GameCode)
            ?? throw new BadRequestException($"Game ({request.GameCode}) is not found or disabled.");

        targetUser = await gamePlatformService.EnsureExternalAccountCreatedAsync(
            targetUser,
            request.GamePlatformId!.Value,
            ct: ct);

        // Login from external API
        var url = await LoginGameAsync(request.GamePlatformId.Value, targetUser.UserExtend!, request, ct)
            ?? throw new BadRequestException($"GamePlatformId({request.GamePlatformId.Value}) is not supported.");

        var gameEmbeddedLink = ConvertEmbeddedLink(request.GamePlatformId.Value, url);
        var loginToken = GenerateToken(gameInfo.PlatformId, gameInfo.Id);
        return new LoginGameResult(gameEmbeddedLink, loginToken);
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
                Passwd = gameAesEncryptor.Decrypt(usrex.GameProviderPassword),
                Gamecode = request.GameCode,
                Address = request.Address,
                Locale = request.Locale,
                ReturnUrl = request.ReturnUrl,
            };
            var result = await gameProvider.LoginAsync(externalRequest, request.IpAddress!);

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

    private string ConvertEmbeddedLink(Guid gamePlatformId, string url)
    {
        if (gamePlatformId == GameConstants.PLATFORM_ID_G598)
        {
            var domain = gameSettings.Value.RevertProxyUrl;
            if (domain.IsNullOrWhiteSpace()) return url;

            var uri = new Uri(url);
            return $"{domain}{uri.PathAndQuery}";
        }

        // Fallback
        return url;
    }

    private static string GenerateToken(Guid platformId, Guid gameId)
    {
        var data = new GameHubTokenDto
        {
            GamePlatformId = platformId,
            GameId = gameId,
        };
        return Base64Helper.Encode(data);
    }
}
