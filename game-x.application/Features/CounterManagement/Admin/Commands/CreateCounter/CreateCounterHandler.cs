using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.CounterManagement.Admin.Commands.CreateCounter;

public sealed class CreateCounterCommandHandler(
    IUnitOfWork unitOfWork,
    ICounterRepo counterRepo,
    IUserAccessor userAccessor) : ICommandHandler<CreateCounterCommand>
{
    public async Task<Unit> Handle(CreateCounterCommand request, CancellationToken ct)
    {
        var isExist = await counterRepo.IsExistCounterNameAsync(request.CounterName, ct);
        if (isExist) throw new InvalidArgumentException(MessageCode.System.DuplicateValue);

        await unitOfWork.WithTransactionAsync(async () =>
        {
            // Create counter-record
            var newCounterNumber = await counterRepo.CreateNewCounterNumberAsync(userAccessor.GetUserId(), ct);
            var counter = Counter.Create(
                counterNumber: newCounterNumber,
                name: request.CounterName,
                location: request.Location,
                desc: request.Description);

            // Create counter-token navigate with the counter
            var counterToken = CounterToken.Create(token: GenerateCounterToken());
            counter.AttachToken(counterToken);

            await counterRepo.AddAsync(counter, ct);
        }, ct);

        return Unit.Value;
    }

    private string GenerateCounterToken()
        => Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("=", string.Empty).Substring(0, 16);
}
