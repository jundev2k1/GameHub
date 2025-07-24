using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.CounterManagement.Admin.Commands.UpdateCounterStatus;

public sealed class UpdateCounterStatusHandler(IUnitOfWork unitOfWork, ICounterRepo counterRepo)
    : ICommandHandler<UpdateCounterStatusCommand>
{
    public async Task<Unit> Handle(UpdateCounterStatusCommand request, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await counterRepo.UpdateAsync(request.CounterId, (counter) =>
            {
                counter.UpdateStatus(request.Status);
            }, ct);
        }, ct);

        return Unit.Value;
    }
}
