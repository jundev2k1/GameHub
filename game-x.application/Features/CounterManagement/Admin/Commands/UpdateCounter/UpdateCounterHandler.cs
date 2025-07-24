using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.CounterManagement.Admin.Commands.UpdateCounter;

public sealed class UpdateCounterHandler(IUnitOfWork unitOfWork, ICounterRepo counterRepo)
    : ICommandHandler<UpdateCounterCommand>
{
    public async Task<Unit> Handle(UpdateCounterCommand request, CancellationToken ct = default)
    {
        var targetCounter = await counterRepo.GetByIdAsync(request.CounterId, ct);
        var isExistName = await counterRepo
            .IsExistNameExcludeIdAsync(request.CounterName, targetCounter.CounterName, ct);
        if (isExistName)
            throw new BadRequestException(MessageCode.System.NameAlreadyExists, "Counter name is existed.");

        await unitOfWork.WithTransactionAsync(async () =>
        {
            await counterRepo.UpdateAsync(request.CounterId, counter =>
            {
                counter.Update(
                    request.CounterName,
                    request.Status,
                    request.Location,
                    request.Description);
            }, ct);
        }, ct);

        return Unit.Value;
    }
}
