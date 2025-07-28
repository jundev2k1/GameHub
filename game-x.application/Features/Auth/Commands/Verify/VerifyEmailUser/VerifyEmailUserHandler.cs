using game_x.application.Contract.Infrastructure.Services.EmailProcessor;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Auth.Commands.Verify.VerifyEmailUser;

public sealed class VerifyEmailUserHandler(
    IEmailVerificationProcessor emailVerification,
    IUserRepo userRepo,
    IUnitOfWork unitOfWork) : ICommandHandler<VerifyEmailUserCommand>
{
    public async Task<Unit> Handle(VerifyEmailUserCommand request, CancellationToken ct = default)
    {
        var isValid = emailVerification.VerifyEmail(request.Email, request.Code);
        if (!isValid) throw new BadRequestException(MessageCode.System.InvalidVerifyCode);

        await userRepo.UpdateByEmailAsync(
            request.Email,
            user => user.ConfirmEmail(),
            ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
