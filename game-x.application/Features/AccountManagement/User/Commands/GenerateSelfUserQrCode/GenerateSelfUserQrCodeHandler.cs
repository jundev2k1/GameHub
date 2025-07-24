using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.AccountManagement.User.Commands.GenerateSelfUserQrCode;

public sealed class GenerateQrCodeCommandHandler(
    IAuthService authService,
    IUserRepo userRepo,
    IUserAccessor userAccessor,
    IUserQrCodeCacheService userQrCodeCacheService) : ICommandHandler<GenerateSelfUserQrCodeCommand, GenerateQrCodeResult>
{
    private readonly int _timeSecondExpiration = 30;

    public async Task<GenerateQrCodeResult> Handle(GenerateSelfUserQrCodeCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var user = await userRepo.GetUserByIdAsync(userId, ct);

        var role = await authService.GetRolesAsync(user);
        if (!role.IsUser)
            throw new ForbiddenException("User is not a normal user.");

        // Calculate expiration time
        var expiresAt = DateTime.UtcNow.AddSeconds(_timeSecondExpiration);
        var qrCodeToken = GenerateRandomQrCodeToken();

        SetCache(userId, qrCodeToken);

        return new GenerateQrCodeResult(
            QrCode: qrCodeToken,
            ExpiresAt: expiresAt.ToString("o"));
    }

    private string GenerateRandomQrCodeToken()
        => Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("=", "").Substring(0, 16);

    private void SetCache(string userId, string token)
    {
        userQrCodeCacheService.SetToken(userId, token);
        userQrCodeCacheService.SetUserId(token, userId);
    }
}
