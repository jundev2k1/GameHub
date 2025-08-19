using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.share.ExternalApi.GameProvider.Dtos.Login;
using game_x.share.ExternalApi.GameProvider.Dtos.Logout;

namespace game_x.application.Features.Games.Commands.LoginGame;

public sealed class LoginGameHandler(
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IGameProviderService gameProvider,
    IGameAesEncryptor aesEncryptor,
    IGameProviderCacheService gameProviderCache) : ICommandHandler<LoginGameCommand, LoginGameResult>
{
    public async Task<LoginGameResult> Handle(LoginGameCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var targetUser = await userRepo.GetUserByIdAsync(userId, ct);
        if (targetUser.UserExtend is null)
            throw new NotFoundException("User extend is not exists.");

        var gameProviderAccount = targetUser.UserExtend.GameProviderAccount;

        // Check: email must be confirmed before requesting password reset
        if (!targetUser.EmailConfirmed)
            throw new BadRequestException(MessageCode.User.UserNotConfirmed);

        // Loggout if user already login
        var isLoggedIn = gameProviderCache.GetIsLoggedIn(gameProviderAccount);
        if (isLoggedIn)
            await LogoutGameAsync(gameProviderAccount);

        // Login from external API
        gameProviderCache.SetLanguage(gameProviderAccount, request.Locale);
        var externalRequest = new LoginRequest
        {
            Account = gameProviderAccount,
            Passwd = aesEncryptor.Decrypt(targetUser.UserExtend.GameProviderPassword),
            Gamecode = request.GameCode,
            Address = request.Address,
            Locale = request.Locale,
            ReturnUrl = request.ReturnUrl,
        };
        var result = await gameProvider.LoginAsync(externalRequest, request.IpAddress!);
        gameProviderCache.SetIsLoggedIn(gameProviderAccount, true);
        return new LoginGameResult(result.Url);
    }

    public async Task LogoutGameAsync(string account)
    {
        var logoutRequest = new LogoutRequest
        {
            Account = account,
        };
        await gameProvider.LogoutAsync(logoutRequest);
    }
}
