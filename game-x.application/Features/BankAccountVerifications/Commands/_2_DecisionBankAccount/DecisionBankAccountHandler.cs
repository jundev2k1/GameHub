using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.BankAccountVerifications.Commands._2_DecisionBankAccount;

public sealed class DecisionBankAccountHandler(
    IUnitOfWork unitOfWork,
    IUserBankAccountRepo bankAccountRepo,
    IUserAccessor userAccessor) : ICommandHandler<DecisionBankAccountCommand>
{
    public async Task<Unit> Handle(DecisionBankAccountCommand request, CancellationToken ct = default)
    {
        var adminId = userAccessor.GetUserId();
        await bankAccountRepo.UpdateAsync(request.Id, targetItem =>
        {
            if (targetItem.Status != UserBankAccountStatus.UnderReview)
                throw new BadRequestException(MessageCode.User.BankAccountInvalid);

            if (request.Status == UserBankAccountStatus.Approved)
                targetItem.Approve(adminId);
            else if (request.Status == UserBankAccountStatus.Rejected)
                targetItem.Reject(adminId, request.Reason!, request.Details!);
            else
                throw new BadRequestException(MessageCode.User.BankAccountStatusInvalid);
        }, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
