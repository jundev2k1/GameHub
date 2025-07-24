using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.BankAccountManagement.Client.Commands.DeleteBankAccount;

public sealed class DeleteBankAccountHandler(
    IBankAccountRepo bankAccountRepo,
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor) : ICommandHandler<DeleteBankAccountCommand>
{
    public async Task<Unit> Handle(DeleteBankAccountCommand request, CancellationToken ct = default)
    {
        var ownerId = userAccessor.GetUserId();
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await bankAccountRepo.DeleteAsync(request.BankAccountCode, ownerId, ct);
        }, ct);

        return Unit.Value;
    }
}
