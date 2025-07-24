using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.BankAccountManagement.Client.Commands.UpdateBankAccount;

public sealed class UpdateBankAccountHandler(
    IUnitOfWork unitOfWork,
    IBankAccountRepo bankAccountRepo,
    IUserAccessor userAccessor) : ICommandHandler<UpdateBankAccountCommand>
{
    public async Task<Unit> Handle(UpdateBankAccountCommand request, CancellationToken ct = default)
    {
        var ownerId = userAccessor.GetUserId();
        var bankAccounts = await bankAccountRepo.GetsByOwnerIdAsync(ownerId, ct);
        await bankAccountRepo.UpdateAsync(request.BankAccountCode, ownerId, bankAccount =>
        {
            bankAccount.Update(
                request.BankAccountNumber ?? bankAccount.BankAccountNumber.Trim(),
                request.BankAccountName ?? bankAccount.BankAccountName.Trim(),
                request.BranchName ?? bankAccount.BranchName.Trim(),
                request.BankName ?? bankAccount.BankName.Trim(),
                request.CurrencyCode is not null ? CurrencyUnit.Of(request.CurrencyCode.Trim()) : bankAccount.CurrencyCode,
                request.AccountType is not null ? AccountType.Of(request.AccountType.Trim()) : bankAccount.AccountType,
                request.Status is not null ? AccountStatus.Of(request.Status.Trim()) : bankAccount.Status);

            var isExist = bankAccounts.Any(ba =>
                ba.PublicId != request.BankAccountCode
                && ba.BankName.ToLowerInvariant() == bankAccount.BankName.ToLowerInvariant()
                && ba.BankAccountName.ToLowerInvariant() == bankAccount.BankAccountName.ToLowerInvariant()
                && ba.BankAccountNumber == bankAccount.BankAccountNumber.Trim()
                && ba.CurrencyCode.Equals(bankAccount.CurrencyCode));
            if (isExist)
                throw new BadRequestException(MessageCode.BankAccount.DuplicateCurrencyInBankAccount);
        }, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
