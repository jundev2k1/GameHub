using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Auth.Dtos;

namespace game_x.application.Features.Auth.Commands.Login.StaffLogin;

public sealed class StaffLoginHandler(
    IJwtTokenGenerator jwtTokenGenerator,
    IAuthService authService,
    ICounterRepo counterRepo,
    IUnitOfWork unitOfWork,
    IStaffCounterRepo staffCounterRepo,
    IHeartBeatCacheService heartBeatCache) : ICommandHandler<StaffLoginCommand, StaffLoginDto>
{
    public async Task<StaffLoginDto> Handle(StaffLoginCommand request, CancellationToken ct = default)
    {
        // Check the existence and validity of the counter
        var targetCounter = await counterRepo.GetByIdAsync(request.CounterId, ct)
            ?? throw new NotFoundException(MessageCode.Counter.CounterNotFound);

        if (!targetCounter.IsActive())
            throw new BadRequestException(MessageCode.Counter.CounterInvalid);

        // Find and validate the staff account
        var loggedUser = await authService.TryLoginAsync(request.UserName, request.Password);
        var (isValid, errorCode) = loggedUser.CheckValidUser();
        if (!isValid) throw new ForbiddenException(errorCode!);

        // Check if the counter is in use
        var isCounterInUse = await staffCounterRepo.IsCounterInUseByAnotherStaffAsync(targetCounter.Id, loggedUser.Id, ct);
        if (isCounterInUse) throw new ForbiddenException(MessageCode.Counter.CounterInUse);

        // Check if the staff is already logged in at another counter
        var isStaffLoggedIn = await staffCounterRepo.IsStaffLoggedInByAnotherCounterAsync(targetCounter.Id, loggedUser.Id, ct);
        if (isStaffLoggedIn) throw new ForbiddenException(MessageCode.Staff.SessionConflict);

        // Check roles to ensure the user is a staff member
        var roles = await authService.GetRolesAsync(loggedUser);
        if (!roles.IsStaff) throw new ForbiddenException();

        // Create a new tracking log and new session for the staff login
        StaffLoginDto? loginDto = null;
        await unitOfWork.WithTransactionAsync(async () =>
        {
            var newStaffCounter = await CreateTrackingLogAsync(loggedUser.Id, request.CounterId, ct);
            var tokenInfo = await jwtTokenGenerator.GenerateToken(loggedUser);

            var newSessionKey = Guid.NewGuid();
            CreateSessionHeartBeat(newStaffCounter, newSessionKey);
            loginDto = new StaffLoginDto
            {
                Token = tokenInfo.Token,
                ExpiresAt = tokenInfo.ExpiresAt,
                UserId = loggedUser.Id,
                UserName = loggedUser.UserName ?? string.Empty,
                Roles = [.. roles],
                CounterId = request.CounterId,
                SessionKey = newSessionKey,
            };
        }, ct);

        return loginDto!;
    }

    private async Task<StaffCounter> CreateTrackingLogAsync(string userId, Guid counterId, CancellationToken ct)
    {
        var targetCounter = await counterRepo.GetByIdAsync(counterId, ct);
        var staffCounter = StaffCounter.Create(
            userId,
            targetCounter.Id);
        await staffCounterRepo.AddTrackingLogAsync(staffCounter, ct);
        return staffCounter;
    }

    private void CreateSessionHeartBeat(StaffCounter staffCounter, Guid sessionKey)
    {
        var sessionId = heartBeatCache.GetCounterHeartBeatKey(staffCounter.Counter.PublicId, staffCounter.UserId, sessionKey);
        heartBeatCache.InsertOrUpdate(
            sessionId,
            () => new HeartBeat.Dtos.HeartBeatDto
            {
                Id = sessionId,
                LastSeenTime = DateTime.UtcNow,
                LoginTime = staffCounter.LoginAt
            });
    }
}
