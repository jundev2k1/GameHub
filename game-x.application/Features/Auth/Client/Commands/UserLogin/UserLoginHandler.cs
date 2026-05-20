using game_x.application.Contract.Infrastructure.BackgroundJobs.Dispatchers;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Events.Account.OnUserLogin;
using game_x.application.Features.Auth.Dtos;
using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;
using game_x.share.Helper;
using RefreshTokenEntity = game_x.domain.Entities.RefreshToken;

namespace game_x.application.Features.Auth.Client.Commands.UserLogin;

public sealed class UserLoginHandler(
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    IJwtTokenGenerator jwtTokenGenerator,
    ITokenService tokenService,
    IRefreshTokenManagerCacheService refreshTokenManager,
    IApplicationEventDispatcher eventDispatcher,
    IUserEventJobDispatcher userEventDispatcher,
    IAuthService authService,
    IUserEventRepo userEventRepo,
    IUserRepo userRepo) : ICommandHandler<UserLoginCommand, UserLoginResult>
{
    public async Task<UserLoginResult> Handle(UserLoginCommand request, CancellationToken ct = default)
    {
        var loginUser = await authService.TryLoginAsync(request.Email, request.Password);
        var (isValid, errorCode) = loginUser.CheckValidUser();
        if (!isValid) throw new ForbiddenException(errorCode!);

        var roles = await authService.GetRolesAsync(loginUser);
        if (roles is {IsUser: false, IsTalent: false}) throw new ForbiddenException();

        if (!loginUser.EmailConfirmed)
            throw new BadRequestException(MessageCode.User.UserNotConfirmed);

        var tokenInfo = await jwtTokenGenerator.GenerateToken(loginUser);
        var refreshToken = tokenService.GenerateRefreshToken(loginUser.Id);

        await RevokePreviousDevice(loginUser.Id);
        CreateRefreshToken(loginUser.Id, refreshToken, tokenInfo.JwtId);

        var loggedUser = await userRepo.GetUserDetailAsync(loginUser.Id, ct);

        await HandleUserEvent(loggedUser.UserId, ct);
        
        return new UserLoginResult(
            Email: loggedUser.Email,
            UserId: loggedUser.UserId,
            Nickname: loggedUser.Nickname,
            AccessToken: tokenInfo.Token,
            RefreshToken: refreshToken.Token,
            Roles: roles.Items,
            AvatarUrl: loggedUser.AvatarUrl);
    }

    private async Task RevokePreviousDevice(string userId)
    {
        refreshTokenManager.GetsByUserId(userId)
            .ToList()
            .ForEach(refreshTokenManager.RevokeToken);

        await eventDispatcher.Publish(new OnUserLoginEvent(userId));
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

    private async Task HandleUserEvent(string userId, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            var id = Guid.CreateVersion7();
            var userEvent = UserEvent.Create(
                userId: userId,
                type: UserEventType.DailyLogin,
                id: id);
            await userEventRepo.AddAsync(userEvent, ct);
            await unitOfWork.SaveChangesAsync(ct);
            userEventDispatcher.EnqueueProcess(id);
         }, ct);
    }
}