using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnVerifyUpdated;
using game_x.application.Features.Accounts.User.Dtos;

namespace game_x.application.Features.BankAccountVerifications.Commands._2_DecisionBankAccount;

public sealed class DecisionBankAccountHandler(
    IUnitOfWork unitOfWork,
    IUserBankAccountRepo bankAccountRepo,
    IFiatCurrencyRepo currencyRepo,
    IApplicationEventDispatcher eventDispatcher,
    IUserAccessor userAccessor) : ICommandHandler<DecisionBankAccountCommand>
{
    public async Task<Unit> Handle(DecisionBankAccountCommand request, CancellationToken ct = default)
    {
        var adminId = userAccessor.GetUserId();
        var userId = string.Empty;
        var currencyId = 0;

        await unitOfWork.WithTransactionAsync(async () =>
        {
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
                userId = targetItem.UserId;
                currencyId = targetItem.CurrencyId;
            }, ct);

            var targetCurrency = await currencyRepo.GetByIdAsync(currencyId, ct);

            var verificationDto = new VerificationStatusDto
            {
                CurrencyCode = targetCurrency?.Code?.Value ?? string.Empty,
                Type = VerificationStatusType.BankAccount,
                Status = (VerificationStatus)(int)request.Status,
                IsVerified = request.Status == UserBankAccountStatus.Approved,
            };

            await eventDispatcher.Publish(new OnVerifyUpdatedEvent(userId, verificationDto), ct);
        }, ct);

        return Unit.Value;
    }


}
