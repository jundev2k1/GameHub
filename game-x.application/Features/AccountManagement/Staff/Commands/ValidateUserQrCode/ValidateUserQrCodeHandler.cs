using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.AccountManagement.Staff.Commands.ValidateUserQrCode;

public sealed class ValidateUserQrCodeCommandHandler(
    IUserRepo userRepo,
    IUserQrCodeCacheService userQrCodeCacheService) : ICommandHandler<ValidateUserQrCodeCommand, ValidateQrCodeResult>
{
    public async Task<ValidateQrCodeResult> Handle(ValidateUserQrCodeCommand request, CancellationToken ct)
    {
        var userId = userQrCodeCacheService.GetUserId(request.QrCode)
            ?? throw new BadRequestException("Invalid or expired token");

        var user = await userRepo.GetUserByIdAsync(userId, ct);
        userQrCodeCacheService.RemoveToken(userId);

        return new ValidateQrCodeResult(
            UserId: user.Id,
            UserName: user.UserName ?? string.Empty,
            Email: user.Email ?? string.Empty,
            PhoneNumber: user.PhoneNumber ?? string.Empty);
    }
}
