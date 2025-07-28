using game_x.application.Contract.Infrastructure.Services.EmailProcessor;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Auth.Commands.ResendCode.ResendCodeUser;

public sealed class ResendCodeUserHandler(
    IEmailVerificationProcessor emailVerification,
    IUserRepo userRepo) : ICommandHandler<ResendCodeUserCommand>
{
    public async Task<Unit> Handle(ResendCodeUserCommand request, CancellationToken ct = default)
    {
        var targetUser = await userRepo.GetUserByEmailAsync(request.Email, ct);
        if (targetUser.EmailConfirmed) throw new BadRequestException(MessageCode.User.EmailAlreadyVerified);

        emailVerification.SendVerificationEmail(request.Email);
        return Unit.Value;
    }
}
