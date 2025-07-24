using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.CounterManagement.Admin.Commands.SoftDeleteCounter;

public sealed class SoftDeleteCounterHandler(ICounterRepo counterRepo, IUnitOfWork unitOfWork)
    : ICommandHandler<SoftDeleteCounterCommand>
{
    public async Task<Unit> Handle(SoftDeleteCounterCommand request, CancellationToken ct)
    {
        await counterRepo.UpdateAsync(request.CounterId, (counter) =>
        {
            if (counter.IsDeleted)
                throw new BadRequestException(MessageCode.System.ItemAlreadyDeleted);

            counter.SoftDelete();
        }, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
