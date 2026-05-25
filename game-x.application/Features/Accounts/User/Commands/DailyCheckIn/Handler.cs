using game_x.application.Common.Abstractions.Time;
using game_x.application.Contract.Infrastructure.BackgroundJobs.Dispatchers;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Accounts.User.Commands.DailyCheckIn;

public sealed class DailyCheckInHandler(
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    IUserEventJobDispatcher userEventDispatcher,
    IUserEventRepo userEventRepo,
    ILogger<DailyCheckInHandler> logger,
    IDateTimeProvider clock
) : ICommandHandler<DailyCheckInCommand, Unit>
{
    public async Task<Unit> Handle(DailyCheckInCommand cmd, CancellationToken ct = default)
    {
        string userId = userAccessor.GetUserId();
        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                var id = Guid.CreateVersion7();
                var userEvent = UserEvent.Create(
                    userId: userId, 
                    type: UserEventType.DailyLogin, 
                    id: id,
                    createdAt: DateTime.SpecifyKind(clock.UtcNow, DateTimeKind.Utc),
                    updatedAt: DateTime.SpecifyKind(clock.UtcNow, DateTimeKind.Utc));
                    
                await userEventRepo.AddAsync(userEvent, ct);
                await unitOfWork.CommitAsync(ct);
                userEventDispatcher.EnqueueProcess(id);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                throw;
            }
        }, ct);
        
        return Unit.Value;
    }
}