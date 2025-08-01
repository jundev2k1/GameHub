using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Kyc.Commands._2_DecisionKyc;

public sealed class DecisionKycHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    IUserAccessor userAccessor) : ICommandHandler<DecisionKycCommand>
{
    public async Task<Unit> Handle(DecisionKycCommand request, CancellationToken ct = default)
    {
        var adminId = userAccessor.GetUserId();
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
        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
