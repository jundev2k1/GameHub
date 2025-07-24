using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.BankAccountManagement.Client.Commands.UpdateDefaultAccount;

public sealed class Handler(IUnitOfWork unitOfWork, IBankAccountRepo bankAccountRepo, IUserAccessor userAccessor)
    : ICommandHandler<UpdateDefaultAccountCommand>
{
    public async Task<Unit> Handle(UpdateDefaultAccountCommand request, CancellationToken ct)
    {
        var ownerId = userAccessor.GetUserId();
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await bankAccountRepo.UpdateDefaultAccountAsync(request.BankAccountCode, ownerId, ct);
        }, ct);

        return Unit.Value;
    }
}
