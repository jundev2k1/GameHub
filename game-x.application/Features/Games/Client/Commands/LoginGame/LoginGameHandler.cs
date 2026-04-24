using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.GameBaccarat;
using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.ExternalApi.IEtl998;
using game_x.application.Contract.Infrastructure.ExternalApi.SasSlot;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.Account.OnUserBalanceUpdated;
using game_x.application.Features.Games.Services;
using game_x.application.Features.UserGameSessions.Dtos;
using game_x.share.Extensions;
using game_x.share.ExternalApi.Etl998.Dtos.ForwardGame;
using game_x.share.ExternalApi.GameBaccarat.Dtos.Login;
using game_x.share.ExternalApi.GameProvider.Dtos.Login;
using game_x.share.Helper;
using game_x.share.Settings;
using Microsoft.Extensions.Options;
using System.Web;
using game_x.application.Contract.Infrastructure.ExternalApi.Atg;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Games.Client.Commands.LoginGame;

public sealed class LoginGameHandler(
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IGameProviderService gameProvider,
    IGameBaccaratService gameBaccarat,
    ISasSlotService sasSlotService,
    IGameAesEncryptor gameAesEncryptor,
    IAesEncryptor aesEncryptor,
    IGameProviderCacheService gameProviderCache,
    IGamePlatformService gamePlatformService,
    IEtl998Service etl998Service,
    IAtgService atgService,
    IOptions<GameProviderSettings> gameSettings,
    IOptions<GameSlotSettings> gameSlotSettings,
    IApplicationEventDispatcher eventDispatcher,
    IOptions<Etl998Settings> settings,
    ILogger<LoginGameHandler> logger) : ICommandHandler<LoginGameCommand, LoginGameResult>
{
    public async Task<LoginGameResult> Handle(LoginGameCommand request, CancellationToken ct = default)
    {
        try
        {
            var userId = userAccessor.GetUserId();
            var targetUser = await userRepo.GetUserByIdAsync(userId, ct);

            // Check: email must be confirmed before requesting password reset
            if (!targetUser.EmailConfirmed)
                throw new BadRequestException(MessageCode.User.UserNotConfirmed);

            var gamPlatform = gameProviderCache.PlatformList.FirstOrDefault(x=> x.Id == request.GamePlatformId);
            if(gamPlatform == null) 
                throw new BadRequestException($"Game ({request.GamePlatformId}) is not found or disabled.");
        
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
            return new LoginGameResult(gameEmbeddedLink, loginToken, gamPlatform.Note);
        }
        catch (BadRequestException ex)
        {
            logger.LogError(ex, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw new BadRequestException(MessageCode.System.SystemError, new { ex.Message });
        }
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
                ReturnUrl = request.ReturnUrl
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
        
        if (gamePlatformId == GameConstants.PLATFORM_ID_ETL998_GAMEBACCARAT)
        {
            var externalRequest = new ForwardGameRequest
            {
                Account = usrex.Etl998ProviderAccount,
                Password = aesEncryptor.Decrypt(usrex.Etl998ProviderPassword),
                Dm = settings.Value.Host
            };
            var result = await etl998Service.ForwardGameAsync(externalRequest);
            var data = result.FirstOrDefault();
            return data?.GameUrl;
        }

        if (gamePlatformId == GameConstants.PLATFORM_ID_SASSLOT)
        {
            var result = await sasSlotService.LoginAsync(usrex.SasSlotAccount, usrex.SasSlotNickname);
            return result;
        }
        
        if (gamePlatformId == GameConstants.PLATFORM_ID_ATG)
        {
            var result = await atgService.PlayGameAsync(usrex.AtgUserName, request.GameCode, usrex.AtgFullname);
            return result;
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

        if (gamePlatformId == GameConstants.PLATFORM_ID_SASSLOT)
        {
            var loginUrl = gameSlotSettings.Value.LoginUrl;
            if (loginUrl.IsNullOrWhiteSpace()) return url;

            var uri = new Uri(url);
            var queryParams = HttpUtility.ParseQueryString(uri.Query);
            var ticket = queryParams.Get("ticket");

            return $"{loginUrl}?ticket={ticket}";
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