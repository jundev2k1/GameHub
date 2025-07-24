using game_x.application.Contract.Infrastructure.Services.EmailProcessor;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.AccountManagement.Staff.Commands.SendVerificationCode;

public sealed class SendVerificationCodeHandler(
    IEmailVerificationProcessor emailVerificationProcessor,
    IUserRepo userRepo) : ICommandHandler<SendVerificationCodeCommand>
{
    public async Task<Unit> Handle(SendVerificationCodeCommand request, CancellationToken ct)
    {
        var user = await userRepo.GetUserByEmailAsync(request.Email, ct);
        if (user.EmailConfirmed)
            throw new BadRequestException(MessageCode.User.EmailAlreadyVerified);

        await emailVerificationProcessor.SendVerificationEmailAsync(request.Email, ct);
        return Unit.Value;
    }
}
