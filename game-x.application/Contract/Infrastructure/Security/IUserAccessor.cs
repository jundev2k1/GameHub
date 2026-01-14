using System.Security.Claims;

namespace game_x.application.Contract.Infrastructure.Security;

public interface IUserAccessor
{
    bool IsLoggedIn();

    string GetUserId();

    string GetJwtId();

    ClaimsPrincipal GetClaimsPrincipal();

    AppRole GetRoles();

    string GetIpAddress();

    public string GetUserAgent();

    public string GetDeviceInfo();
}
