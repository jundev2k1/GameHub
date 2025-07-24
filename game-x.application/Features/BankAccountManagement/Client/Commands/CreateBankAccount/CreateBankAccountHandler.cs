using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.BankAccountManagement.Client.Commands.CreateBankAccount;

namespace game_x.application.Features.BankAccountManagement.Client.Commands.CreateBankAccount;

public sealed class Handler(
    IUnitOfWork unitOfWork,
    IBankAccountRepo bankAccountRepo,
    IUserAccessor userAccessor) : ICommandHandler<CreateBankAccountCommand>
{
    public async Task<Unit> Handle(CreateBankAccountCommand request, CancellationToken ct)
    {
        var ownerId = userAccessor.GetUserId();
        var bankAccounts = await bankAccountRepo.GetsByOwnerIdAsync(ownerId, ct);
        var isExist = bankAccounts.Any(ba =>
            ba.BankName.ToLowerInvariant() == request.BankName.ToLowerInvariant()
            && ba.BankAccountName.ToLowerInvariant() == request.BankAccountName.ToLowerInvariant()
            && ba.BankAccountNumber == request.BankAccountNumber.Trim()
            && ba.CurrencyCode.Equals(CurrencyUnit.Of(request.CurrencyCode)));
        if (isExist)
            throw new BadRequestException(MessageCode.BankAccount.DuplicateCurrencyInBankAccount);

        var hasDefault = bankAccounts.Any(ba => ba.IsDefault);
        var bankAccount = BankAccount.Create(
            accountNumber: request.BankAccountNumber,
            name: request.BankAccountName,
            branchName: request.BranchName,
            bankName: request.BankName,
            currencyCode: CurrencyUnit.Of(request.CurrencyCode),
            type: AccountType.Of(request.AccountType),
            status: AccountStatus.Of(request.Status),
            ownerId: ownerId,
            isDefault: !hasDefault);
        await bankAccountRepo.AddAsync(bankAccount, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
