using game_x.domain.Identity;
using System.Security.Claims;

namespace game_x.application.Contract.Infrastructure.Security;

public interface IUserAccessor
{
    string GetUserId();

    ClaimsPrincipal GetClaimsPrincipal();

    AppRole GetRoles();
}
