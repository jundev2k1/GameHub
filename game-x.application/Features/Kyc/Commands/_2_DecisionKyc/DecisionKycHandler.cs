using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnVerifyUpdated;
using game_x.application.Features.Accounts.User.Dtos;

namespace game_x.application.Features.Kyc.Commands._2_DecisionKyc;

public sealed class DecisionKycHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    IApplicationEventDispatcher eventDispatcher,
    IUserAccessor userAccessor) : ICommandHandler<DecisionKycCommand>
{
    public async Task<Unit> Handle(DecisionKycCommand request, CancellationToken ct = default)
    {
        var adminId = userAccessor.GetUserId();

        await unitOfWork.WithTransactionAsync(async () =>
        {
            await userRepo.UpdateKycAsync(request.UserId, targetKyc =>
            {
                if (targetKyc.Status != KycStatus.UnderReview)
                    throw new BadRequestException(MessageCode.User.KycInvalidStatus);

                if (request.Status == KycStatus.Approved)
                    targetKyc.Approve(adminId);
                else if (request.Status == KycStatus.Rejected)
                    targetKyc.Reject(adminId, request.Reason, request.Details);
                else
                    throw new BadRequestException(MessageCode.User.KycInvalidStatus);
            }, ct);

            var verificationDto = new VerificationStatusDto
            {
                CurrencyCode = string.Empty,
                Type = VerificationStatusType.Kyc,
                Status = (VerificationStatus)(int)request.Status,
                IsVerified = request.Status == KycStatus.Approved
            };

            await eventDispatcher.Publish(new OnVerifyUpdatedEvent(request.UserId, verificationDto), ct);
        }, ct);

        return Unit.Value;
    }
}
