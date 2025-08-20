using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Events.OnUserLogin;
using game_x.application.Features.Auth.Dtos;
using game_x.share.Extensions;
using game_x.share.Helper;
using RefreshTokenEntity = game_x.domain.Entities.RefreshToken;

namespace game_x.application.Features.Auth.Client.Commands.UserLogin;

public sealed class UserLoginHandler(
    IUserAccessor userAccessor,
    IJwtTokenGenerator jwtTokenGenerator,
    ITokenService tokenService,
    IRefreshTokenManagerCacheService refreshTokenManager,
    IAuthService authService,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<UserLoginCommand, UserLoginResult>
{
    public async Task<UserLoginResult> Handle(UserLoginCommand request, CancellationToken ct = default)
    {
        var loginUser = await authService.TryLoginAsync(request.Email, request.Password);
        var (isValid, errorCode) = loginUser.CheckValidUser();
        if (!isValid) throw new ForbiddenException(errorCode!);

        var roles = await authService.GetRolesAsync(loginUser);
        if (!roles.IsUser) throw new ForbiddenException();

        if (!loginUser.EmailConfirmed)
            throw new BadRequestException(MessageCode.User.UserNotConfirmed);

        var tokenInfo = await jwtTokenGenerator.GenerateToken(loginUser);
        var refreshToken = tokenService.GenerateRefreshToken(loginUser.Id);
        CreateRefreshToken(loginUser.Id, refreshToken, tokenInfo.JwtId);
        RevokeAllTokenSameDevice(loginUser.Id, refreshToken.Token);

        _ = eventDispatcher.Publish(new OnUserLoginEvent(loginUser.Id), ct);

        return new UserLoginResult(
            Email: loginUser.Email!,
            UserId: loginUser.Id,
            Nickname: loginUser.Nickname,
            Token: tokenInfo.Token,
            RefreshToken: refreshToken.Token);
    }

    private void CreateRefreshToken(string userId, RefreshTokenGenerateDto tokenInfo, string jwtId)
    {
        var token = RefreshTokenEntity.Create(
            userId,
            HashHelper.Sha256(tokenInfo.Token),
            jwtId,
            tokenInfo.ExpiresAt,
            userAccessor.GetIpAddress(),
            userAccessor.GetUserAgent(),
            deviceInfo: userAccessor.GetDeviceInfo());
        refreshTokenManager.InsertNewToken(token.Adapt<RefreshTokenDto>());
    }

    private void RevokeAllTokenSameDevice(string userId, string newToken)
    {
        var ipAddress = userAccessor.GetIpAddress();
        var deviceInfo = userAccessor.GetDeviceInfo();
        var hashToken = HashHelper.Sha256(newToken);
        var sameDeviceTokenIds = refreshTokenManager.GetsByUserId(userId)
            .Where(rt => !rt.IsRevoked
                && !rt.IsExpired
                && rt.IpAddress.Equals(ipAddress, StringComparison.InvariantCultureIgnoreCase)
                && rt.DeviceInfo.ToStringOrEmpty().Equals(deviceInfo, StringComparison.InvariantCultureIgnoreCase)
                && rt.TokenHash != hashToken)
            .Select(rt => rt.TokenHash)
            .ToList();
        sameDeviceTokenIds.ForEach(
            token => refreshTokenManager.RevokeToken(userId, token));
    }
}
